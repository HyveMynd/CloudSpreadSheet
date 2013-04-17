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
    {}
    
    
    /*
     * Called from the controller. Will find the correct spreadsheet and call the
     * join method on the correct spreadsheet.
     */
    ss_result server::do_join(std::string name, std::string password)
    {}
    
    /*
     * Called from the controller. Wraps the changes in a 'cell' object and calls the 
     * 'change' method on the correct spreadsheet.
     */
    ss_result server::do_change(std::string data, int version)
    {}
    
    /*
     * Called from the controller. Wraps the changes in a 'cell' object and calls the
     * 'update' method on the correct spreadsheet.
     */
    ss_result server::do_update(std::string data, int version)
    {}
    
    /*
     * Called from the controller. Find the correct spreadsheet and calls the 
     * undo method for that spreadsheet.
     */
    ss_result server::do_undo(std::string name, int version)
    {}
    
    /*
     * Called from the controller. Find the correct spreadsheet and calls the
     * save method for that spreadsheet.
     */
    ss_result server::do_save(std::string name)
    {}
    
    /*
     * Called from the controller. Find the correct spreadsheet and calls the
     * leave method for that spreadsheet.
     */
    ss_result server::do_leave(std::string name)
    {}
    
}