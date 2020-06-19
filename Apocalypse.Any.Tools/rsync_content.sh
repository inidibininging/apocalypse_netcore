#!/bin/bash
# rsync built content with the executable
APOCALYPSE_DIR=$HOME/Documents/apocalypse
APOCALYPSE_NETCORE_DIR=$APOCALYPSE_DIR/Apocalypse.Any.NetCore
SOURCEDIR=$APOCALYPSE_NETCORE_DIR/Presentation/Apocalypse.Any.Client/Content/bin/Windows/
DESTDIR=$APOCALYPSE_NETCORE_DIR/Presentation/Apocalypse.Any.Client/bin/Debug/netcoreapp2.1/Content/

rsync -rv $SOURCEDIR $DESTDIR

