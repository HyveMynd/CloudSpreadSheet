#include <cstdlib>
#include <iostream>
#include <string>
#include "parse.h"
using namespace std; 


int main() 
{
   string command = "Name:untitled01";
   string command2 = "CREATE\nName:name\nPassword:password\n";
    
   string name = "";
   string password = "";
   
     if(get_word(3,command2,'\n').substr(0,9) == "Password:")
      {
	 name = get_word(3,command2,'\n').substr(9,(get_word(3,command2,'\n')).size()-9);
      }
 
   cout << name << endl;

   return 0;
}
