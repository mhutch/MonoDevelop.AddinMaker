CONFIG?=Debug

all:
	xbuild /p:Configuration=${CONFIG} ${ARGS}

clean:
	xbuild /t:Clean ${ARGS}

install:
	xbuild /p:InstallAddin=True /p:Configuration=${CONFIG} ${ARGS}
