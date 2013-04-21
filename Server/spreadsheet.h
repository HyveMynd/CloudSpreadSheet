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
#include <map>
#include <boost/bind.hpp>
#include <boost/asio.hpp>
#include "ss_result.h"
#include "cell.h"
#include "user.h"
#include "enums.h"
#include "parse.h"

namespace serverss {

    class spreadsheet{
  
    
    public:
        
        // Constructors
        spreadsheet(std::string, std::string);
        
        // Spreadsheet functions
        ss_result join(user*, ss_result&);
        ss_result change(cell, int, ss_result&);
        ss_result save(ss_result&);
        void leave(user*);
        ss_result undo(int, ss_result&);
    
        // Accessors
        void get_user(std::string);
        void get_all_users();
        int get_version_number();
        std::string get_password();
    
    private:
    
        std::list<user*> users;
        std::stack<cell> undo_stack;
        std::string password;
        std::string name;
        std::map<std::string, std::string> ss_contents;
        int version;
        
        void log(std::string);
        void update(ss_result result, user* user_to_update);
        user* find_user(user*);
        ss_result& incorrect_version_error(ss_result&);
        ss_result& make_error(ss_result&, std::string);
        
//        void updateConfirmation(const boost::system::error_code& error,
//                                size_t bytes_transferred);
//        void sendUpdate(boost::asio::ip::tcp::socket *socket_, string message_);
    };

}

#endif /* defined(____spreadsheet__) */
