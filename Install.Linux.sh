#!/bin/bash
mkdir temp
cd temp
git clone https://github.com/creeperlv/SimpleCSharpCompiler.git
cd SimpleCSharpCompiler
dotnet build
mv -f SimpleCSharpCompiler/bin/Debug/netcoreapp3.1/ /usr/share/SimpleCSharpCompiler/
sudo ln -s /usr/share/SimpleCSharpCompiler/scsc /usr/bin/scsc
cd ..
rm -rf temp