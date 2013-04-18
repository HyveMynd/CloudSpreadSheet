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
        std::stringstream ver;
        ver << version;
        
        switch (status){
            case OK:
                return ok_to_string();
            case FAIL:
                return error_to_string();
            case WAIT:
                return wait_to_string();
            case END:
                return "UNDO END\nName"+file_name+"\nVersion:"+ver.str()+"\n";
        }

        
    }
    
    std::string ss_result::error_to_string(){
        std::string error = " FAIL\nName:"+file_name+"\n"+message+"\n";
        
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
    
    std::string ss_result::ok_to_string(){
        std::stringstream ver;
        ver << version;
        
        switch (command) {
            case Create:
                return "CREATE OK\nName"+file_name+"\nPassword:"+file_password+"\n";
            case Join:
                return "JOIN OK\nName"+file_name+"\nVersion:"+ver.str()+"\nLength:"+length+"\n"+xml+"\n";
            case Change:
                return "CHANGE OK\nName"+file_name+"\nVersion:"+ver.str()+"\n";
            case Update:
                return "UPDATE\nName:"+file_name+"\nVersion:"+ver.str()+"\nCell:"+cell_result.to_string()+"\nLength:"+length+"\n"+contents+"\n";
            case Undo:
                return "UNDO OK\nName:"+file_name+"\nVersion:"+ver.str()+"\nCell:"+cell_result.to_string()+"\nLength:"+length+"\n"+contents+"\n";
            case Save:
                return "SAVE OK\nName:"+file_name+"\n";
            default:
                throw 1;
        }
    }
    
    std::string ss_result::wait_to_string(){
        std::stringstream ver;
        ver << version;
        std::string wait = " WAIT\nName:"+file_name+"\nVersion:"+ver.str()+"\n";
        
        switch (command) {
            case Change:
                return "CHANGE "+wait;
            case Update:
                return "UPDATE "+wait;
            case Undo:
                return "UNDO "+wait;
            default:
                throw 1;
        }
    }
    

}
