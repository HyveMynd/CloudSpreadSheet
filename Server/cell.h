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
        
        cell(std::string);
        cell();
        
        std::string get_position();
        std::string get_contents();
        std::string to_string();
        
    private:
        
        std::string position;
        std::string contents;
        
    };
    
}

#endif /* defined(____cell__) */
