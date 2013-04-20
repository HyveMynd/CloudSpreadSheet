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
        this->version = 0;
    }
    
    /*
     * Adds the user to the list of users.
     */
    ss_result spreadsheet::join(user* new_user, ss_result& result)
    {
        users.push_back(new_user);
        result.version = version;
        
        // Check the list to see if the user is already joined
        if (find_user(new_user) == NULL)
            return make_error(result, "User has already joined the editing session.");
        
        // Add user to the list of users
        users.push_back(new_user);
        
        result.xml = get_xml(name);
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
        
        
        std::map<std::string, std::string>::iterator it = ss_contents.find(changes.cell_name);
        
        if (cll == NULL)
        {
            cell old;
            old.cell_name = changes.cell_name;
            undo_stack.push(old);
            ss_contents.insert(std::pair<std::string, std::string>(changes.cell_name, changes.contents));
        }
        else
        {
            cell old = (*cll);
            undo_stack.push(old);
            cll->contents = changes.contents;
        }
        
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
        
        // Nothing to undo
        if (undo_stack.size() == 0)
        {
            result.status = END;
            return result;
        }
        
//        cell undo_cell = undo_stack.top();
//        undo_stack.pop();
//        
//        cell* cll = find_cell(undo_cell.cell_name);
//        
//        cll->contents = undo_cell.contents;
        
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
        put_xml(name, ss_contents);
        
        result.status = OK;
        return result;
    }
    
    /*
     * Removes the user from the user list
     */
    void spreadsheet::leave(user user_leaving)
    {
        //do leave
        
    }
    
    void spreadsheet::log()
    {}
    
    user* spreadsheet::find_user(user* this_user)
    {
        for (std::list<user*>::iterator it = users.begin(); it != users.end(); it++)
        {
            if ((*it) == this_user)
                return (*it);
        }
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