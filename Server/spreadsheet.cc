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
    ss_result spreadsheet::join(user new_user)
    {}
    
    /*
     * Checks the user version number against the spreadsheet version number.
     * If equal the changes will go through and the version number will increase.
     * otherwise fail.
     */
    ss_result spreadsheet::change(cell changes, int user_version)
    {}
    
    /*
     * Checks the user version against the spreadsheet version. If
     * equal the updates will go through and the version increases. 
     * Otherwise fail.
     */
    ss_result spreadsheet::update(cell updates, int user_version)
    {}
    
    /*
     * Checks the user version agaisnt the spreadsheet version. If equal,
     * the last change on the stack is popped, applied, and the version 
     * increased. Otherwise fail.
     */
    ss_result spreadsheet::undo(int user_version)
    {}
    
    /*
     * Saves the spreadsheet out to a file on the disk.
     */
    ss_result spreadsheet::save()
    {}
    
    /*
     * Removes the user from the user list
     */
    ss_result spreadsheet::leave(user user_leaving)
    {}
    
    void spreadsheet::log()
    {}
    
}