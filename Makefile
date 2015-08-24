CONFIG=Debug

all:
	xbuild /p:Configuration=${CONFIG}

clean:
	xbuild /t:Clean

install:
	xbuild /p:InstallAddin=True /p:Configuration=${CONFIG}
