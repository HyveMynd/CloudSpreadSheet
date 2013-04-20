/* Writtent by Bill Ford
 * cs 3505 
 * Spreadsheet parse functions 
 */

#include "parse.h"
#include <string>
#include <iostream>
#include <fstream>
#include <map>
#include <stack>
#include <cstring>

using namespace std;

namespace serverss {

  // gets the indexed word from the line for parsing the data 
  string get_word(int word_index, string str, char ch) {
    int sp_index = 1;
    string word;  

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


  /* return the token between "<" and ">" and 
   * removes the token from string s */
  string gettoken(string &s) 
  {
    int left = s.find_first_of("<");
    int right = s.find_first_of(">");
    string result = s.substr(left + 1, right -left - 1);
    cout << "\nleft=" << left << "\nright=" << right << "\ntoken=" << result << endl;
    s = s.substr(right + 1);
    return result;
  }
  
  string getvalue(string &s) {
    int left = s.find_first_of("<");
    string result = s.substr(0, left -1);
    s = s.substr(left +1);

    return result;
  }

  /* returns a hash map of cells read from
   * filename on disk */
  map<string,string> get_map(string fname)
  {
    map<string, string> result;
    map<string, string>::iterator it;
    stack<string> tokens;
    enum states {spreadsheet, tokenstate, cell, name, contents };

    // header removed from xml
    // format is validated in get_xml
    string s = get_xml(fname);
   
    int state = tokenstate;
    string token = "";
    string cellname = "";
    string cellcontents ="";

    int i = 0;
    while (!s.empty()) {
      cout << "\ns=" << s << "\ntoken=" << token << "\ncellname=" << cellname << "\ncontents=" << contents << endl;
      if (i++ > 10) break; 
      switch (state)  {
       case tokenstate : 
           token = uppercase(gettoken(s)); 
           if (token == "CELL") {
             state = cell;
             continue;
           }
           else if (token == "NAME") {
             state = name;
             continue;
           }
           else if (token == "CONTENTS")  {
             state = contents;
             continue;
           }
           else if (token[0] == '/') {
             state = tokenstate;
             continue;
           }
           break;
       case cell :
         state = tokenstate;
         token = gettoken(s);
         continue;
         break;
       case name :
         cout << "getting cellname " << endl <<  s << endl;
         cellname  = getvalue(s);
         state = tokenstate;
         continue;
       case contents: 
         cellcontents = getvalue(s);
         it = result.find(cellname);

         if (it != result.end())
             result.insert(pair<string,string>(cellname, cellcontents));

         cellname = "";
         cellcontents = "";
         state = tokenstate;
         break;
      } //switch
    } // while
    return result;
  } // get_map


  /* change string to upper case */
  string uppercase(string s) 
    {
      for (int i; i < s.size(); i++)
        s[i] = toupper(s[i]);
      return s;

    }

  const string  header = "<?XML VERSION=\"1.0\" ENCODING=\"UTF-8\"?><SPREADSHEET VERSION=\"PS6\">";

  /* check format checks the validity of an xml string
   * the file format */
  bool checkformat(string &s)
  {
    s = uppercase(s); 
    if (s.find(header) == string::npos)
      return false;
    s = s.substr(header.size()); 
    
    return true;
  }

  

  /* returns a xml formatted string of cells */
  string get_xml(string fname) {
    string result = "";

    ifstream is(fname.c_str()); 

    while (is.good())
    {
      char c = is.get();
      if (is.good())
        cout << c;
        result += c;
    }
    is.close();                

    if (checkformat(result)) 
        return result;
    else return NULL;
  }

  /* creates an xml formatted string and saves it to disk */
  bool put_xml(const string fname, map<string, string> data)
  { 
    bool result = false;
    fstream ss;
    ss.open(fname.c_str(), fstream::out | fstream::app);
    //ss.open(fname.c_str(), fstream::out | fstream::app | fstream::trunc);

    ss << header;
    
    // iterate throught the data
    map<string,string>::iterator it;
    for (map<string,string>::iterator it=data.begin(); it!=data.end(); ++it)
      ss << "<cell><name>" << it->first << "</name><contents>" << it->second << "</contents><cell>";
   
    ss << "</spreadsheet>";
    ss.close();
    return true;
  }
}


