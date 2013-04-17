//
//  controller.h
//  
//
//  Created by Andres Monroy on 4/15/13.
//
//

#ifndef ____controller__
#define ____controller__

#include <iostream>
#include <string>
#include "server.h"

namespace serverss{

    class controller{
    
    public:


    private:

        std::string do_get();
        std::string do_create();
        std::string do_join();
        std::string do_change();
        std::string do_undo();
        std::string do_update();
        std::string do_save();
        std::string do_leave();
        void log();
        
        server my_server;
        
    };
}

#endif /* defined(____controller__) */
