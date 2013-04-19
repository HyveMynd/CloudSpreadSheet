#include "parse.h"
#include <string>
#include <iostream>

using namespace std;

namespace serverss {

  // gets the indexed word from the line for parsing the data 
  std::string get_word(int word_index, std::string str, char ch) {
    int sp_index = 1;
    std::string word;  

    for (int i = 0; i < str.length(); i++) {
  
    if (sp_index > word_index) { 
      return word; 
    } // if
  
    if (str[i] == ch || str[i] == '\r') { 
      sp_index++;
    } // if
    else if (sp_index == word_index) {
      word.push_back(str[i]);
    } // else if
  
    } // for
    return word;
  }

  // returns a hash map of cells read from
  // filename on disk
  map<string,string> get_map(string fname)
  {
    map<string, string> result;
    return result;
  }

  // returns a xml formatted string of cells
  string get_xml(string fname)
  {
    string result = "";
    return result;
  }

  // creates an xml formatted string and saves it to disk
  bool put_xml(string fname, map<string, string> xml)
  { 
    
    bool result = false;
    return result;
  }
}


