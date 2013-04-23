#ifndef PARSE_H
#define PARSE_H
#include <iostream>
#include <string>
#include <map>

namespace serverss {


  // header for all spreadsheets 
  const std::string  header = "<?xml version=\"1.0\" encoding=\"utf-8\"?><spreadsheet/>";
  const std::string  header1 = "<?xml version=\"1.0\" encoding=\"utf-8\"?><spreadsheet>";
  //const std::string  header = "<?xml version=\"1.0\" encoding=\"utf-8\"?><spreadsheet version=\"ps6\"/>";
  const std::string ssdb = "data/ss.db";

  void log(std::string s, std::string msg);

  // gets the indexed word from the line for parsing the data
  std::string get_word(int word_index, std::string str, char ch);

  // returns a hashmap of the spreadsheet
  std::map<std::string,std::string> get_map(std::string fname);

  // get_xml_from_map
  std::string get_xml_from_map(std::map<std::string, std::string> data);

  // returns a xml string of a spreadsheet
  std::string get_xml(std::string fname);

  // saves a file to disk in xml format
  bool put_xml(std::string fname, std::map<std::string, std::string> xml);

  /* change string to lower case */
  std::string lowercase(std::string s);

  /*
   * get_file_password
   */
  std::string get_file_password(std::string fname);

  /*
   * put_file_password
   */
  bool put_file_password(std::string fname, std::string password);
} 
#endif 
