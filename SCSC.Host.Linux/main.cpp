#include <iostream>
#include <string>
using namespace std;
int main(int argc,char *argv[])
{
    FILE* config = fopen("Runtime.Config","R+");
    
    char* dllFile=new char[256];
    fgets(dllFile,255,config);
     
    string cmdLine = "dotnet ";
    cmdLine += dllFile;
    cmdLine += " ";
    if (argc > 1) {
        for (int i = 1; i < argc; i++)
        {
            cmdLine += "\"";
            cmdLine += argv[i];
            cmdLine += "\"";
        }
    }
    system(cmdLine.c_str());
    return 0;
}