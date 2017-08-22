CONFIG?=Debug
SLNFILE=MonoDevelop.AddinMaker.sln

all: restore
	msbuild ${SLNFILE} /p:Configuration=${CONFIG} ${ARGS}

clean:
	msbuild ${SLNFILE} /p:Configuration=${CONFIG} /t:Clean ${ARGS}

install: restore
	msbuild ${SLNFILE} /p:InstallAddin=True /p:Configuration=${CONFIG} ${ARGS}

restore:
	msbuild ${SLNFILE} /t:Restore /p:Configuration=${CONFIG} ${ARGS}

.PHONY: all clean install restore
