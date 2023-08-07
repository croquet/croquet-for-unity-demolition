#!/bin/bash
cd `dirname "$0"`

npm run build-three

SOURCE=dist
TARGET=../../../../../wonderland/servers/croquet-io-dev/demolition-multi

rm -rf $TARGET
mkdir $TARGET
cp "$SOURCE"/* $TARGET
