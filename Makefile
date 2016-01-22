CONFIG?=Debug

all: restore
	xbuild /p:Configuration=${CONFIG} ${ARGS}

clean:
	xbuild /t:Clean ${ARGS}

install: restore
	xbuild /p:InstallAddin=True /p:Configuration=${CONFIG} ${ARGS}

restore:
	nuget restore

.PHONY: all clean install restore
