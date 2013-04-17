//
//  cell.h
//  
//
//  Created by Andres Monroy on 4/16/13.
//
//

#ifndef ____cell__
#define ____cell__

#include <iostream>
#include <string>

namespace serverss{
    
    class cell{
      
    public:
        
        std::string get_position();
        std::string get_contents();
        
    private:
        
        std::string position;
        std::string contents;
        
    };
    
}

#endif /* defined(____cell__) */
