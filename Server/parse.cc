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

  map<string,string> get_map(string fname)
  {
    map<string, string> result;
    return result;
  }
  string get_xml(string fname)
  {
    string result = "";
    return result;
  }
  string put_xml(map<string, string> xml)
  { 
    string result = "";
    return result;
  }
}


