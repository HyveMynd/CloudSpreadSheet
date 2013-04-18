//
//  spreadsheet.h
//  
//
//  Created by Andres Monroy on 4/16/13.
//
//

#ifndef ____spreadsheet__
#define ____spreadsheet__

#include <iostream>
#include <string>
#include <list>
#include <stack>
#include <fstream>
#include "ss_result.h"
#include "cell.h"
#include "user.h"

namespace serverss {

    class spreadsheet{
  
    
    public:
        
        // Constructors
        spreadsheet(std::string, std::string);
        
        // Spreadsheet functions
        ss_result join(user, ss_result&);
        ss_result change(cell, int, ss_result&);
        ss_result update(cell, int, ss_result&);
        ss_result save(ss_result&);
        ss_result leave(user, ss_result&);
        ss_result undo(int, ss_result&);
    
        // Accessors
        void get_user(std::string);
        void get_all_users();
        int get_version_number();
        std::string get_password();
    
    private:
    
        std::list<user> users;
        std::stack<cell> undo_stack;
        std::string password;
        std::string name;
        int version;
        
        void log();
        ss_result& incorrect_version_error(ss_result&);
        ss_result& make_error(ss_result&, std::string);
    };

}

#endif /* defined(____spreadsheet__) */
