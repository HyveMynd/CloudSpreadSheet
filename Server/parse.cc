#include "parse.h"
#include <string>
#include <iostream>

using namespace std;

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



