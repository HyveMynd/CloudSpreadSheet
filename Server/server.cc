//
//  server.cc
//
//
//  Created by Andres Monroy on 4/16/13.
//
//

#include "server.h"


namespace serverss{
    
    server::server()
    {
        
    }
    
    /* 
     * Called from the controller. Will check if the spreadsheet exists, and 
     * create the spreadsheet if it does not exist. Adds the spreadsheet to 
     * the list of spreadsheets.
     */
    ss_result server::do_create(std::string name, std::string password)
    {
        // TODO check for spaces
        name = add_extension(name);
        ss_result result;
        result.file_name = name;
        result.file_password = password;
        std::map<std::string, spreadsheet>::iterator it = spreadsheets.find(name);

        // If the spreadsheet does not exist, create one and insert into the map.
        // return the name and password
        if (it == spreadsheets.end() && !file_exists(name)){
            spreadsheets.insert(std::pair<std::string, spreadsheet>(name, spreadsheet(name, password)));
            result.status = OK;
            result.command = Create;
            
            return result;
        }
        
        return make_error(result, "File name already exists. Please choose a different name.");
    }
    
    
    /*
     * Called from the controller. Will find the correct spreadsheet and 
     * ensure the user can join the spreadsheet.
     */
    ss_result server::do_join(std::string name, std::string password, user* new_user)
    {
        name = add_extension(name);
        std::map<std::string, spreadsheet>::iterator it = spreadsheets.find(name);
        ss_result result;
        result.command = Join;
        result.file_name = name;
        result.file_password = password;
        
        // TODO Add to map if necesary
        
        if (it == spreadsheets.end() && !file_exists(name) )
            return not_found_error(result);
        
        if (it->second.get_password().compare(password) != 0)
            return make_error (result, "The password for the file is incorrect. Try again."); 
        
        return it->second.join(new_user, result);
    }
    
    /*
     * Called from the controller. Wraps the changes in a 'cell' object and calls the 
     * 'change' method on the correct spreadsheet.
     */
    ss_result server::do_change(std::string name, int version, cell new_cell)
    {
        ss_result result;
        result.file_name = name;
        result.command = Change;
        spreadsheet* ss = find_ss(name);

        if (ss == NULL)
            return not_found_error(result);
        
        return ss->change(new_cell, version, result);
    }
    
    /*
     * Called from the controller. Wraps the changes in a 'cell' object and calls the
     * 'update' method on the correct spreadsheet.
     */
    ss_result server::do_update(std::string name, int version, cell new_cell)
    {
        ss_result result;
        result.file_name = name;
        result.command = Update;
        spreadsheet* ss = find_ss(name);
        
        if (ss == NULL)
            return not_found_error(result);
        
        return ss->update(new_cell, version, result);
    }
    
    /*
     * Called from the controller. Find the correct spreadsheet and calls the 
     * undo method for that spreadsheet.
     */
    ss_result server::do_undo(std::string name, int version)
    {
        ss_result result;
        result.file_name = name;
        result.command = Undo;
        spreadsheet* ss = find_ss(name);
        
        
        if (ss == NULL)
            return not_found_error(result);
        
        return ss->undo(version, result);
    }
    
    /*
     * Called from the controller. Find the correct spreadsheet and calls the
     * save method for that spreadsheet.
     */
    ss_result server::do_save(std::string name)
    {
        ss_result result;
        result.file_name = name;
        result.command = Save;
        spreadsheet* ss = find_ss(name);
        
        
        if (ss == NULL)
            return not_found_error(result);
        
        return ss->save(result);
    }
    
    /*
     * Called from the controller. Find the correct spreadsheet and calls the
     * leave method for that spreadsheet.
     */
    void server::do_leave(std::string name, user user_leaving)
    {
        ss_result result;
        result.file_name = name;
        result.command = Leave;
        spreadsheet* ss = find_ss(name);
        
        ss->leave(user_leaving);
    }

    ss_result& server::make_error(ss_result& result, std::string message)
    {
        result.status = FAIL;
        result.message = message;
        return result;
    }
    
    spreadsheet* server::find_ss(std::string file_name)
    {
        std::map<std::string, spreadsheet>::iterator it = spreadsheets.find(file_name);
        spreadsheet* ss = NULL;
        
        if (it != spreadsheets.end())
            ss = &(it->second);
        return ss;
    }
    
    ss_result& server::not_found_error(ss_result& result)
    {
        return make_error(result, "Could not find the spreadsheet");
    }
    
    void server::log()
    {}
    
    
    // Function: fileExists
    /**
     Check if a file exists
     @param[in] filename - the name of the file to check
     
     @return    true if the file exists, else false
     
     */
    bool server::file_exists(std::string& filename)
    {
        std::string fn;
        struct stat buf;
        fn = add_extension(filename);

        return stat(fn.c_str(), &buf) != 1;
    }
    
    std::string server::add_extension(std::string& filename)
    {
        if(filename.substr(filename.find_last_of(".") + 1) == "ss")
            return filename;
        else
            return filename + ".ss";
    }
    
}
