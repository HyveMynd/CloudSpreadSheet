//
//  user.cc
//  
//
//  Created by Andres Monroy on 4/16/13.
//
//

#include "user.h"


namespace serverss {
    
    int user::id_count = 0;
    
    user::user(boost::asio::ip::tcp::socket* user_socket)
    {
        this->uid = id_count;
        id_count++;
        this->user_socket = user_socket;
        this->valid = true;
    }
    
}