#!/bin/bash -e

if test -n "${MONO_STATIC_AOT}";
then
${TESTCMD} --label=mkbundle --timeout=25m make -j ${CI_CPU_COUNT} -w -C mcs/class/corlib -k mkbundle-all-tests
fi

if test -n "${MONO_LLVMONLY}";
then
${TESTCMD} --label=mini --timeout=25m make -j ${CI_CPU_COUNT} -w -C mono/mini -k llvmonlycheck
else
${TESTCMD} --label=mini --timeout=25m make -j ${CI_CPU_COUNT} -w -C mono/mini -k fullaotcheck
if [[ ${CI_TAGS} == *'_llvm'* ]]; then
	${TESTCMD} --label=mini-aotcheck --timeout=25m make -j ${CI_CPU_COUNT} -w -C mono/mini -k llvmaotcheck
	# FIXME: https://github.com/mono/mono/issues/15999
	# ${TESTCMD} --label=mini-aotcheck --timeout=25m make -j ${CI_CPU_COUNT} -w -C mono/mini -k llvmfullaotcheck
fi
fi

${TESTCMD} --label=runtime --timeout=160m make -w -C mono/tests -k test-wrench V=1
${TESTCMD} --label=corlib --timeout=30m make -w -C mcs/class/corlib run-test
${TESTCMD} --label=verify --timeout=15m make -w -C runtime mcs-compileall
${TESTCMD} --label=profiler --timeout=30m make -w -C mono/profiler -k check
${TESTCMD} --label=System --timeout=10m make -w -C mcs/class/System run-test
${TESTCMD} --label=System.XML --timeout=5m make -w -C mcs/class/System.XML run-test
${TESTCMD} --label=Mono.Security --timeout=5m make -w -C mcs/class/Mono.Security run-test
${TESTCMD} --label=System.Security --timeout=15m make -w -C mcs/class/System.Security run-test V=1
${TESTCMD} --label=System.Data --timeout=5m make -w -C mcs/class/System.Data run-test
${TESTCMD} --label=System.Web.Services --timeout=5m make -w -C mcs/class/System.Web.Services run-test
${TESTCMD} --label=I18N.CJK --timeout=5m make -w -C mcs/class/I18N/CJK run-test
${TESTCMD} --label=I18N.West --timeout=5m make -w -C mcs/class/I18N/West run-test
${TESTCMD} --label=I18N.MidEast --timeout=5m make -w -C mcs/class/I18N/MidEast run-test
${TESTCMD} --label=I18N.Rare --timeout=5m make -w -C mcs/class/I18N/Rare run-test
${TESTCMD} --label=I18N.Other --timeout=5m make -w -C mcs/class/I18N/Other run-test
${TESTCMD} --label=System.Transactions --timeout=5m make -w -C mcs/class/System.Transactions run-test
${TESTCMD} --label=System.Core --timeout=15m make -w -C mcs/class/System.Core run-test
${TESTCMD} --label=System.Xml.Linq --timeout=5m make -w -C mcs/class/System.Xml.Linq run-test
${TESTCMD} --label=System.Runtime.Serialization --timeout=5m make -w -C mcs/class/System.Runtime.Serialization run-test
${TESTCMD} --label=System.ServiceModel --timeout=15m make -w -C mcs/class/System.ServiceModel run-test
${TESTCMD} --label=System.ServiceModel.Web --timeout=5m make -w -C mcs/class/System.ServiceModel.Web run-test
${TESTCMD} --label=System.ComponentModel.DataAnnotations --timeout=5m make -w -C mcs/class/System.ComponentModel.DataAnnotations run-test
${TESTCMD} --label=Mono.CSharp --timeout=5m make -w -C mcs/class/Mono.CSharp run-test
${TESTCMD} --label=System.Numerics --timeout=5m make -w -C mcs/class/System.Numerics run-test
${TESTCMD} --label=System.Net.Http --timeout=5m make -w -C mcs/class/System.Net.Http run-test
${TESTCMD} --label=System.Json --timeout=5m make -w -C mcs/class/System.Json run-test
${TESTCMD} --label=monolinker --timeout=10m make -w -C mcs/tools/linker check

rm -fr /tmp/jenkins-temp-aspnet*

${MONO_REPO_ROOT}/scripts/ci/run-upload-sentry.sh
