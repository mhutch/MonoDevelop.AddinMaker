CONFIG?=Debug

all: restore
	msbuild /p:Configuration=${CONFIG} ${ARGS}

clean:
	msbuild /t:Clean ${ARGS}

install: restore
	msbuild /p:InstallAddin=True /p:Configuration=${CONFIG} ${ARGS}

restore:
	msbuild /t:Restore /p:Configuration=${CONFIG} ${ARGS}

.PHONY: all clean install restore
