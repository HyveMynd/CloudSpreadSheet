//
//  spreadsheet.cc
//  
//
//  Created by Andres Monroy on 4/16/13.
//
//

#include "spreadsheet.h"


namespace serverss{
    
    /*--------Constructors----------*/
    spreadsheet::spreadsheet(std::string name, std::string password)
    {
        this->name = name;
        this->password = password;
        this->version = 1;
    }
    
    /*
     * Adds the user to the list of users.
     */
    ss_result spreadsheet::join(user new_user, ss_result& result)
    {
        users.push_back(new_user);
        result.version = version;
        
        //do join
        
        result.status = OK;
        return result;
    }
    
    /*
     * Checks the user version number against the spreadsheet version number.
     * If equal the changes will go through and the version number will increase.
     * otherwise fail.
     */
    ss_result spreadsheet::change(cell changes, int user_version, ss_result& result)
    {
        if (user_version != version)
            return incorrect_version_error(result);
        
        //do change
        
        version++;
        result.version = version;
        result.status = OK;
        return result;
    }
    
    /*
     * Checks the user version against the spreadsheet version. If
     * equal the updates will go through and the version increases. 
     * Otherwise fail.
     */
    ss_result spreadsheet::update(cell updates, int user_version, ss_result& result)
    {
        if (user_version != version)
            return incorrect_version_error(result);
        
        //do update
        
        version++;
        result.version = version;
        result.status = OK;
        return result;
    }
    
    /*
     * Checks the user version agaisnt the spreadsheet version. If equal,
     * the last change on the stack is popped, applied, and the version 
     * increased. Otherwise fail.
     */
    ss_result spreadsheet::undo(int user_version, ss_result& result)
    {
        if (user_version != version)
            return incorrect_version_error(result);
        
        //do undo
        
        version++;
        result.version = version;
        result.status = OK;
        return result;
    }
    
    /*
     * Saves the spreadsheet out to a file on the disk.
     */
    ss_result spreadsheet::save(ss_result& result)
    {
        //do save
        
        result.status = OK;
        return result;
    }
    
    /*
     * Removes the user from the user list
     */
    ss_result spreadsheet::leave(user user_leaving, ss_result& result)
    {
        //do leave
        
        result.status = OK;
        return result;
    }
    
    void spreadsheet::log()
    {}
    
    user* spreadsheet::find_user(user& this_user)
    {
        return NULL;
    }
    
    ss_result& spreadsheet::make_error(ss_result& result, std::string message)
    {
        result.status = FAIL;
        result.message = message;
        return result;
    }
    
    ss_result& spreadsheet::incorrect_version_error(ss_result& result)
    {
        return make_error(result, "Incorrect Version Number");
    }
    
}