#!/bin/bash
source build_vars.sh

echo 'content:' $CONTENT_DIR
echo 'destination:' $DEST_DIR
echo 'deleting destination first...'
rm -R $DEST_DIR

mkdir $DEST_DIR

echo 'starting build'
dotnet tool run mgcb-editor $CONTENT_DIR/Content.mgcb 
mv $OUTPUT_DIR $DEST_DIR
