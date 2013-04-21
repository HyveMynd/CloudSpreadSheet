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
        log("Entered join");
        result.version = version;
        
        // Check the list to see if the user is already joined
        if (find_user(new_user) != NULL)
            return make_error(result, "User has already joined the editing session.");
        
        // Add user to the list of users
        log("Adding user to list");
        users.push_back(new_user);
        
        log("Getting map from parser");
        std::string fname = "data/" + name;
        ss_contents = get_map(fname);
        
        log("Getting xml from file");
        result.xml = get_xml(fname);
        log("XML is: " + result.xml);
        result.length = result.xml.length();
        
        result.status = OK;
        log("Join success");
        return result;
    }
    
    /*
     * Checks the user version number against the spreadsheet version number.
     * If equal the changes will go through and the version number will increase.
     * otherwise fail.
     */
    ss_result spreadsheet::change(cell changes, int user_version, ss_result& result)
    {
        log("Entered change");
        if (user_version != version)
            return incorrect_version_error(result);
        
        result.cell_result = changes;
        result.version = user_version;
        
        std::map<std::string, std::string>::iterator cll = ss_contents.find(changes.cell_name);
        cell old;

        version++;
        if (cll == ss_contents.end())
        {
            log("New cell. Adding to map");
            old.cell_name = changes.cell_name;
            old.contents = "";
            undo_stack.push(old);
            ss_contents.insert(std::pair<std::string, std::string>(changes.cell_name, changes.contents));
        }
        else
        {
            log("Cell exists. Changing contents");

            old.cell_name = cll->first;
            old.contents = cll->second;
            undo_stack.push(old);
            cll->second = changes.contents;
            
            
//            //update all users
//            log("Sending updates to users");
//            for (std::list<user>::iterator it = users.begin(); it != users.end(); ++it)
//            {
//                update(result, (*it));
//            }
        }
        log("map has new cell changes");
        result.contents = changes.contents;
        result.version = version;
		result.status = OK;
        log("Returning change success");
        return result;
    }
    
    void spreadsheet::update(ss_result result, user* user_to_update)
    {
        log("Sending Update");
        result.command = Update;
//        sendUpdate(user->user_socket, result.to_string());
    }
    
    /*
     * Checks the user version agaisnt the spreadsheet version. If equal,
     * the last change on the stack is popped, applied, and the version 
     * increased. Otherwise fail.
     */
    ss_result spreadsheet::undo(int user_version, ss_result& result)
    {
        log("Entered undo");
        if (user_version != version)
            return incorrect_version_error(result);
        
        // Nothing to undo
        if (undo_stack.size() == 0)
        {
            log("Nothing to undo");
            result.status = END;
            return result;
        }
        
        cell undo_cell = undo_stack.top();
        undo_stack.pop();
        log("Undoing cell " + undo_cell.cell_name);
        std::map<std::string, std::string>::iterator cll = ss_contents.find(undo_cell.cell_name);
        
        cll->second = undo_cell.contents;
        
        version++;
        result.version = version;
        result.status = OK;
        log("Undo Success");
        return result;
    }
    
    /*
     * Saves the spreadsheet out to a file on the disk.
     */
    ss_result spreadsheet::save(ss_result& result)
    {
        log("Entered save");
        std::string fname = "data/" + name;
        put_xml(fname, ss_contents);
        
        result.status = OK;
        log("Save Success");
        return result;
    }
    
    /*
     * Removes the user from the user list
     */
    bool spreadsheet::leave(user* user_leaving)
    {
        log("Entered leave");
        user* leave = find_user(user_leaving);
        
        if (leave == NULL)
            return false;
        
        log("Leave Success");
        users.remove(user_leaving);
        
        return users.size() == 0;
    }
    
    void spreadsheet::log(std::string message)
    {
        if(ss_log)
        	std::cout << "SPREADSHEET LOG " << name << ": " << message << std::endl;
    }
    
    user* spreadsheet::find_user(user* this_user)
    {
        for (std::list<user*>::iterator it = users.begin(); it != users.end(); it++)
        {
            if ((*it)->uid == this_user->uid)
                return (*it);
        }
        return NULL;
    }
    
    ss_result& spreadsheet::make_error(ss_result& result, std::string message)
    {
        log("ERROR: " + message);
        result.status = FAIL;
        result.message = message;
        return result;
    }
    
    ss_result& spreadsheet::incorrect_version_error(ss_result& result)
    {
        return make_error(result, "Incorrect Version Number");
    }
    
    
    
    void updateConfirmation(const boost::system::error_code& error,
                            size_t bytes_transferred)
    {
        std::cout << "update Sent" << std::endl;
    }
    
    void sendUpdate(boost::asio::ip::tcp::socket *socket_, std::string message_)
    {
     
         boost::asio::async_write((*socket_),
                                  boost::asio::buffer(message_),
                                  updateConfirmation);
     
    }
    
    /*-------Accesors--------*/
    void spreadsheet::get_user(std::string)
    {
        
    }
  void spreadsheet::get_all_users()
    {
        
    }
    int spreadsheet::get_version_number()
    {
        return version;
    }
    std::string spreadsheet::get_password()
    {
        return password;
    }


    
}
