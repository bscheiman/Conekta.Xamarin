#!/bin/bash

if [ -z "$1" ]
  then
    echo "Version argument required"
    exit
fi

rm *.nupkg 2>/dev/null
rm /Users/bscheiman/NuGet/AppCreator*.nupkg 2>/dev/null

xbuild /t:Clean
xbuild /t:Rebuild /p:Configuration=Release
nuget pack Conekta.Xamarin.nuspec -Version $1 -Verbosity detailed

cp -f *.nupkg /Users/bscheiman/NuGet/

if [ "$2" = "push" ]; then
  nuget push Conekta.Xamarin.$1.nupkg
fi
