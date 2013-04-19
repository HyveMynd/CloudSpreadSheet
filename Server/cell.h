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
        
        std::string cell_name;
        std::string value;
        
        std::string to_string();
        
    private:
        
        void parse_contents(std::string);
        
    };
    
}

#endif /* defined(____cell__) */
