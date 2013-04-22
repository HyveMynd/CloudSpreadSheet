//
//  server.cc
//
//
//  Created by Andres Monroy on 4/16/13.
//
//

#include "server.h"

namespace serverss{
    
    /* 
     * Called from the controller. Will check if the spreadsheet exists, and 
     * create the spreadsheet if it does not exist. Adds the spreadsheet to 
     * the list of spreadsheets.
     */
    ss_result server::do_create(std::string name, std::string password)
    {
        log("Entered do_create");
        // TODO check for spaces
        name = add_extension(name);
        ss_result result;
        result.file_name = name;
        result.file_password = password;
        result.command = Create;
        spreadsheet* ss = find_ss(name);

        // If the spreadsheet does not exist, create one and insert into the map.
        // return the name and password
        if (ss == NULL && !file_exists(name)){
            log (name + " spreadsheet is not found. Creating it");
            spreadsheets.insert(std::pair<std::string, spreadsheet>(name, spreadsheet(name, password)));
            log("Added " + name + " to map.");
            result.status = OK;
            
            // Create the file on disk
            log("Writng " + name + " to disk:");
            std::string filename = "data/"+name;
            std::ofstream outfile (filename.c_str());
            //outfile << "<?XML VERSION=\"1.0\" ENCODING=\"UTF-8\"?><SPREADSHEET VERSION=\"PS6\"/>";
            outfile << header;
            outfile.close();
            
            //put file password
//            put_file_password(filename);
            
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
        log("Entered do_join");
        std::stringstream sss;
        sss << new_user->uid;
        log("Uid is: " + sss.str());
        name = add_extension(name);
        ss_result result;
        result.command = Join;
        result.file_name = name;
        result.file_password = password;
        
        //add ss to map if neccesary
		spreadsheet* ss = find_ss(name);
//        if (ss == NULL)
//        {
//            std::string filename = "data/"+name;
//            std::string file_password = get_file_password(filename);
//            
//            if (file_password.compare("") == 0)
//                return make_error (result, "The password for the file is incorrect. Try again.");
//            
//            std::map<std::string, std::string> cell_map = get_map(filename);
//            spreadsheets.insert(std::pair<std::string, spreadsheet>(name, spreadsheet(name, password, cell_map) ));
//        }
        
        if (ss == NULL)
            return not_found_error(result);
        
        if (ss->get_password().compare(password) != 0)
            return make_error (result, "The password for the file is incorrect. Try again."); 
        
        return ss->join(new_user, result);
    }
    
    /*
     * Called from the controller. Wraps the changes in a 'cell' object and calls the 
     * 'change' method on the correct spreadsheet.
     */
    ss_result server::do_change(std::string name, int version, cell new_cell)
    {
        name = add_extension(name);
        log("Entered do_change. File to modify: " +name);
        ss_result result;
        result.file_name = name;
        result.command = Change;
        spreadsheet* ss = find_ss(name);
        if (ss == NULL)
            return not_found_error(result);
        
        return ss->change(new_cell, version, result);
    }
    
    /*
     * Called from the controller. Find the correct spreadsheet and calls the 
     * undo method for that spreadsheet.
     */
    ss_result server::do_undo(std::string name, int version)
    {
        log("Entered do_undo");
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
        log("Entered do_save");
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
    void server::do_leave(std::string name, user* user_leaving)
    {
        log("Entered do_leave");
        ss_result result;
        result.file_name = name;
        result.command = Leave;
        spreadsheet* ss = find_ss(name);
        
        if(ss == NULL)
        {
            log("SS not found to remove user");
			return;
        }
            
        //remove spreadsheet from memory if all users have left
        if(ss->leave(user_leaving))
        {
            log("no more users. Removing ss from map");
            spreadsheets.erase(name);
        }
    }

    ss_result& server::make_error(ss_result& result, std::string message)
    {
        log("ERROR: " + message);
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
    
    void server::log(std::string message)
    {
        if(server_log)
        {
            std::string logstr = "SERVER LOG: " +  message;
            std::string logfname = "data/log.txt";
        	std::cout << logstr << std::endl;
            std::ofstream logfile(logfname.c_str());
            logfile << logstr << "\n";
            logfile.close();   
        }
    }
    
    
    // Function: fileExists
    /**
     Check if a file exists
     @param[in] filename - the name of the file to check
     
     @return    true if the file exists, else false
     
     */
    bool server::file_exists(std::string filename)
    {
        add_extension(filename);
        filename = "data/" + filename;
        const char* fn = filename.c_str();
        std::ifstream ifile(fn);
        return ifile;
    }
    
    std::string server::add_extension(std::string filename)
    {
//        if(filename.substr(filename.find_last_of(".") + 1) == "ss")
//            return filename;
//        else
//            return filename + ".ss";
        return filename;
    }
    
}
