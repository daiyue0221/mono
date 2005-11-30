//
// ListDictionaryCas.cs - CAS unit tests for 
//	System.Collections.Specialized.ListDictionary
//
// Author:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using NUnit.Framework;

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MonoTests.System.Collections.Specialized {

	public class UnitTestNameObjectCollectionBase: NameObjectCollectionBase {

		public UnitTestNameObjectCollectionBase ()
		{
		}

		public UnitTestNameObjectCollectionBase (int capacity)
			: base (capacity)
		{
		}

		public UnitTestNameObjectCollectionBase (IHashCodeProvider provider, IComparer comparer)
			: base (provider, comparer)
		{
		}

		public UnitTestNameObjectCollectionBase (int capacity, IHashCodeProvider provider, IComparer comparer)
			: base (capacity, provider, comparer)
		{
		}

#if NET_2_0
		public UnitTestNameObjectCollectionBase (IEqualityComparer comparer)
			: base (comparer)
		{
		}

		public UnitTestNameObjectCollectionBase (int capacity, IEqualityComparer comparer)
			: base (capacity, comparer)
		{
		}
#endif
		public bool _IsReadOnly {
			get { return base.IsReadOnly; }
			set { base.IsReadOnly = value; }
		}

		public void Add (string name, object value)
		{
			base.BaseAdd (name, value);
		}

		public void Clear ()
		{
			base.BaseClear ();
		}

		public object Get (int index)
		{
			return base.BaseGet (index);
		}

		public string[] GetAllKeys ()
		{
			return base.BaseGetAllKeys ();
		}

		public object[] GetAllValues ()
		{
			return base.BaseGetAllValues ();
		}

		public object[] GetAllValues (Type type)
		{
			return base.BaseGetAllValues (type);
		}

		public string GetKey (int index)
		{
			return base.BaseGetKey (index);
		}

		public bool HasKeys ()
		{
			return base.BaseHasKeys ();
		}

		public void Remove (string name)
		{
			base.BaseRemove (name);
		}

		public void RemoveAt (int index)
		{
			base.BaseRemoveAt (index);
		}

		public void Set (int index, object value)
		{
			base.BaseSet (index, value);
		}

		public void Set (string name, object value)
		{
			base.BaseSet (name, value);
		}
	}

#if NET_2_0
	public class EqualityComparer: IEqualityComparer {

		public bool Equals (object x, object y)
		{
			return (CaseInsensitiveComparer.DefaultInvariant.Compare (x, y) == 0);
		}

		public int GetHashCode (object obj)
		{
			return obj.GetHashCode ();
		}
	}
#endif

	[TestFixture]
	public class NameObjectCollectionBaseTest {

		private void CheckICollection (UnitTestNameObjectCollectionBase coll, int count)
		{
			ICollection collection = (coll as ICollection);
			Assert.AreEqual (count, collection.Count, "Count");
			Assert.IsFalse (collection.IsSynchronized, "IsSynchronized");
			Assert.IsNotNull (collection.SyncRoot, "SyncRoot");
			string[] array = new string[count];
			collection.CopyTo (array, 0);
			for (int i = 0; i < count; i++) {
				Assert.AreEqual (coll.GetKey (i), array[i], "#" + i.ToString ());
			}
		}

		[Test]
		public void Constructor_Default ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase ();
			Assert.AreEqual (0, coll.Count, "Count-0");
			Assert.IsFalse (coll._IsReadOnly, "IsReadOnly");
			Assert.IsFalse (coll.HasKeys (), "HasKeys-0");

			coll.Add ("a", "1");
			CheckICollection (coll, 1);
			Assert.AreEqual (1, coll.Count, "Count-1");
			Assert.AreEqual ("1", coll.Get (0), "Get(0)");
			Assert.AreEqual (1, coll.GetAllKeys ().Length, "GetAllKeys");
			Assert.IsTrue (coll.HasKeys (), "HasKeys-1");

			coll.Add ("b", "2");
			Assert.AreEqual (2, coll.Count, "Count-2");
			Assert.AreEqual ("b", coll.GetKey (1), "GetKey(1)");
			Assert.AreEqual (2, coll.GetAllValues ().Length, "GetAllValues");

			coll.Remove ("a");
			Assert.AreEqual (1, coll.Count, "Count-3");

			coll.Set (0, "3");
			Assert.AreEqual ("3", coll.Get (0), "Get(0)b");
			coll.Set ("b", "4");
			Assert.AreEqual ("4", coll.Get (0), "Get(0)c");

			coll.RemoveAt (0);
			Assert.AreEqual (0, coll.Count, "Count-4");
			Assert.IsFalse (coll.HasKeys (), "HasKeys-2");
		}

		[Test]
		public void Constructor_Int ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase (0);
			for (int i = 0; i < 10; i++)
				coll.Add (i.ToString (), i);

			CheckICollection (coll, 10);
			Assert.AreEqual (10, coll.Keys.Count, "Keys");

			coll.Clear ();
			Assert.AreEqual (0, coll.Count, "Count");
			Assert.IsFalse (coll.HasKeys (), "HasKeys");
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void Constructor_Int_MinValue ()
		{
			new UnitTestNameObjectCollectionBase (Int32.MinValue);
		}
#if NET_2_0
		[Test]
		public void Constructor_IEqualityComparer ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase (new EqualityComparer ());
			coll.Add ("a", "1");
			CheckICollection (coll, 1);
		}

		[Test]
		public void Constructor_Int_IEqualityComparer ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase (5, new EqualityComparer ());
			coll.Add ("a", "1");
			CheckICollection (coll, 1);
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void Constructor_IntNegative_IEqualityComparer ()
		{
			new UnitTestNameObjectCollectionBase (-1, new EqualityComparer ());
		}

		[Test]
		public void GetObjectData_IEqualityComparer ()
		{
			EqualityComparer comparer = new EqualityComparer ();
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase (5, comparer);
			coll.Add ("a", "1");
			coll.Add ("b", "2");
			coll._IsReadOnly = true;

			SerializationInfo si = new SerializationInfo (typeof (UnitTestNameObjectCollectionBase), new FormatterConverter ());
			coll.GetObjectData (si, new StreamingContext ());
			foreach (SerializationEntry se in si) {
				switch (se.Name) {
				case "KeyComparer":
					Assert.AreSame (comparer, se.Value, se.Name);
					break;
				case "ReadOnly":
					Assert.IsTrue ((bool) se.Value, se.Name);
					break;
				case "Count":
					Assert.AreEqual (2, se.Value, se.Name);
					break;
				case "Values":
					Assert.AreEqual (2, (se.Value as object[]).Length, se.Name);
					break;
				case "Keys":
					Assert.AreEqual (2, (se.Value as string[]).Length, se.Name);
					break;
				case "Version":
					Assert.AreEqual (4, se.Value, se.Name);
					break;
				default:
					string msg = String.Format ("Unexpected {0} information of type {1} with value '{2}'.",
						se.Name, se.ObjectType, se.Value);
					Assert.Fail (msg);
					break;
				}
			}
		}
#endif
		[Test]
		public void Constructor_Provider_Comparer ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase (CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
			coll.Add (null, null);
			Assert.AreEqual (1, coll.Count, "Count-1");
			coll.Remove (null);
			Assert.AreEqual (0, coll.Count, "Count-0");
		}

		[Test]
		public void Constructor_Int_Provider_Comparer ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase (5, CaseInsensitiveHashCodeProvider.DefaultInvariant, CaseInsensitiveComparer.DefaultInvariant);
			coll.Add ("a", "1");
			int i = 0;
			IEnumerator e = coll.GetEnumerator ();
			while (e.MoveNext ()) {
				i++;
			}
			Assert.AreEqual (1, i, "GetEnumerator");
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void Constructor_Int_Negative ()
		{
			new UnitTestNameObjectCollectionBase (-1, CaseInsensitiveHashCodeProvider.DefaultInvariant, CaseInsensitiveComparer.DefaultInvariant);
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void GetObjectData_Null ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase ();
			coll.GetObjectData (null, new StreamingContext ());
		}

		[Test]
		public void GetObjectData ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase (CaseInsensitiveHashCodeProvider.DefaultInvariant, CaseInsensitiveComparer.DefaultInvariant);
			coll.Add ("a", "1");

			SerializationInfo si = new SerializationInfo (typeof (UnitTestNameObjectCollectionBase), new FormatterConverter ());
			coll.GetObjectData (si, new StreamingContext ());
			foreach (SerializationEntry se in si) {
				switch (se.Name) {
				case "HashProvider":
					Assert.AreSame (CaseInsensitiveHashCodeProvider.DefaultInvariant, se.Value, se.Name);
					break;
				case "Comparer":
					Assert.AreSame (CaseInsensitiveComparer.DefaultInvariant, se.Value, se.Name);
					break;
				case "ReadOnly":
					Assert.IsFalse ((bool)se.Value, se.Name);
					break;
				case "Count":
					Assert.AreEqual (1, se.Value, se.Name);
					break;
				case "Values":
					Assert.AreEqual (1, (se.Value as object[]).Length, se.Name);
					break;
				case "Keys":
					Assert.AreEqual (1, (se.Value as string[]).Length, se.Name);
					break;
				case "Version":
					Assert.AreEqual (2, se.Value, se.Name);
					break;
				default:
					string msg = String.Format ("Unexpected {0} information of type {1} with value '{2}'.",
						se.Name, se.ObjectType, se.Value);
					Assert.Fail (msg);
					break;
				}
			}
		}

		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void Add_ReadOnly ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase ();
			coll._IsReadOnly = true;
			coll.Add ("a", "1");
		}

		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void Clear_ReadOnly ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase ();
			coll._IsReadOnly = true;
			// even if we're empty
			coll.Clear ();
		}

		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void Remove_ReadOnly ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase ();
			coll.Add ("a", "!");
			coll._IsReadOnly = true;
			coll.Remove ("a");
		}

		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void RemoveAt_ReadOnly ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase ();
			coll.Add ("a", "!");
			coll._IsReadOnly = true;
			coll.RemoveAt (0);
		}

		[Test]
#if NET_2_0
		[ExpectedException (typeof (NotSupportedException))]
#else
		[Ignore ("Read-only is ignored for Set() under MS 1.x.")]
#endif
		public void Set_ReadOnly ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase ();
			coll.Add ("a", "!");
			coll._IsReadOnly = true;
			coll.Set (0, "1");
			// no exception before 2.0
			Assert.AreEqual ("1", coll.Get (0), "Get(0)"); // :-(
		}

		[Test]
		[ExpectedException (typeof (ArrayTypeMismatchException))]
		public void GetAllValues_Type_Mismatch ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase ();
			coll.Add ("a", "string");
			coll.Add ("b", Int32.MinValue);
			string[] array = (string[]) coll.GetAllValues (typeof (string));
			Assert.AreEqual (1, array.Length, "Length");
			Assert.AreEqual ("string", array[0], "[0]");
		}

		[Test]
		public void GetAllValues_Type ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase ();
			coll.Add ("a", "string1");
			coll.Add ("b", "string2");
			string[] array = (string[]) coll.GetAllValues (typeof (string));
			Assert.AreEqual (2, array.Length, "Length");
			Assert.AreEqual ("string1", array[0], "[0]");
			Assert.AreEqual ("string2", array[1], "[1]");
		}

		[Test]
		public void GetAllValues ()
		{
			UnitTestNameObjectCollectionBase coll = new UnitTestNameObjectCollectionBase ();
			coll.Add ("a", "string1");
			coll.Add ("b", "string2");
			object[] array = (object[]) coll.GetAllValues ();
			Assert.AreEqual (2, array.Length, "Length");
			Assert.AreEqual ("string1", array[0], "[0]");
			Assert.AreEqual ("string2", array[1], "[1]");
		}
	}
}
