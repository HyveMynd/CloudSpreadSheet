//
//  ss_result.h
//  
//
//  Created by Andres Monroy on 4/16/13.
//
//

#ifndef ____ss_result__
#define ____ss_result__

#include <iostream>
#include <string>
#include <sstream>
#include <list>
#include "cell.h"
#include "user.h"
#include "enums.h"

namespace serverss{
    
    class ss_result{
      
    public:
        
        ss_result();
        
        
        std::string to_string();
        
        
        // Member Variables
        Command command;
        std::list<user> send_list;
        std::string file_name;
        std::string file_password;
        std::string message;
        int version;
        int length;
        std::string xml;
        cell cell_result;
        std::string contents;
        Status status;
        
        
    private:
        
        std::string error_to_string();
        std::string ok_to_string();
        std::string wait_to_string();
        
    };
    
}

#endif /* defined(____ss_result__) */
