//
//  server.h
//  
//
//  Created by Andres Monroy on 4/16/13.
//
//

#ifndef _server_h
#define _server_h

#include <sys/stat.h>
#include <map>
#include <string>
#include <iostream>
#include <fstream>
#include "parse.h"
#include "enums.h"
#include "user.h"
#include "spreadsheet.h"
#include "ss_result.h"
#include "cell.h"

namespace serverss {

    class server{
    
    public:
        
        // Constructors
    
        ss_result do_create(std::string, std::string);
        ss_result do_join(std::string, std::string, user*);
        ss_result do_change(std::string, int, cell);
        ss_result do_undo(std::string, int);
        ss_result do_save(std::string);
        void do_leave(std::string, user*);
    
    private:
    
        std::map<std::string, spreadsheet> spreadsheets;
        const static bool server_log = true;
        
        void log(std::string);
        ss_result& make_error (ss_result&, std::string);
        spreadsheet* find_ss(std::string);
        ss_result& not_found_error(ss_result&);
        bool file_exists(std::string);
        std::string add_extension(std::string);
    };
    
}


#endif