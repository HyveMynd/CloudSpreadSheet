#include <iostream>
#include <map>

namespace serverss {

  // gets the indexed word from the line for parsing the data
  std::string get_word(int word_index, std::string str, char ch);
  std::map<std::string,std::string> get_map(std::string fname);
  std::string get_xml(std::string fname);
  std::string put_xml(std::map<std::string, std::string> xml);

} 
