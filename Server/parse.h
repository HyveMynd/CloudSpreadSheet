#ifndef PARSE_H
#define PARSE_H
#include <iostream>
#include <string>
#include <map>

namespace serverss {

  // gets the indexed word from the line for parsing the data
  std::string get_word(int word_index, std::string str, char ch);

  // returns a hashmap of the spreadsheet
  std::map<std::string,std::string> get_map(std::string fname);

  // returns a xml string of a spreadsheet
  std::string get_xml(std::string fname);

  // saves a file to disk in xml format
  bool put_xml(std::string fname, std::map<std::string, std::string> xml);

  /* change string to upper case */
  std::string uppercase(std::string s);

} 
#endif 
