#!/bin/bash
cd `dirname "$0"`

if [ $1 == "" ] ; then echo "Must supply a version number for deployment (e.g., 1.4)" ; exit 1 ; fi

npm run build-three

SOURCE=build-tools/dist
VERSION=$1
TARGET=../../../../../wonderland/servers/croquet-io-dev/demolition-multi-$VERSION

rm -rf $TARGET
mkdir $TARGET
cp "$SOURCE"/* $TARGET
