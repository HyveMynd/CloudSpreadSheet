//
//  ss_result.cc
//  
//
//  Created by Andres Monroy on 4/16/13.
//
//

#include "ss_result.h"

namespace serverss {
    ss_result::ss_result(){
        
    }
    
    std::string ss_result::to_string(){
        if (success){
            
        }
        else
            return error_to_string();
    }
    
    std::string ss_result::error_to_string(){
        
        std::string error = " FAIL\nNAME:"+file_name+"\n"+message;
        
        switch (command) {
            case Create:
                return "CREATE"+error;
            case Join:
                return "JOIN"+error;
            case Change:
                return "CHANGE"+error;
            case Update:
                return "UPDATE"+error;
            case Undo:
                return "UNDO"+error;
            case Save:
                return "SAVE"+error;
            case Leave:
                return "LEAVE"+error;
            default:
                throw 1;
        }
    }
}
