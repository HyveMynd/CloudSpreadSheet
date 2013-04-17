//
//  server.h
//  
//
//  Created by Andres Monroy on 4/16/13.
//
//

#ifndef _server_h
#define _server_h

#include <map>
#include <string>
#include "spreadsheet.h"
#include "ss_result.h"
#include "cell.h"
#include <iostream>

namespace serverss {

    class server{
    
    public:
        
        // Constructors
        server();
    
        ss_result do_create(std::string, std::string);
        ss_result do_update(std::string, int);
        ss_result do_join(std::string, std::string);
        ss_result do_change(std::string, int);
        ss_result do_undo(std::string, int);
        ss_result do_save(std::string);
        ss_result do_leave(std::string);
    
    private:
    
        std::map<std::string, spreadsheet> spreadsheets;
        void log();

    };
    
}


#endif
