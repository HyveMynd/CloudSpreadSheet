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

  int debug = 0;
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
    if (left == -1 || right == -1) {
      s = "";
      return "";
    }

    string result = s.substr(left, right -left+1);
    if (debug) cout << "\nleft=" << left << "\nright=" << right << "\ntoken=" << result << endl;
    s = s.substr(right + 1);
    return result;
  }
  
  string getvalue(string &s) {
    int left = s.find_first_of("<");
    string result = s.substr(0, left);
    s = s.substr(left);

    return result;
  }

  /* returns a hash map of cells read from
   * filename on disk */
  map<string,string> get_map(string fname)
  {
    map<string, string> result;
    map<string, string>::iterator it;
    string cellname = "";
    string cellcontents = "";
    stack<string> tokens;
    enum states {spreadsheet, tokenstate, cell, name, contents };

    // header removed from xml
    // format is validated in get_xml
    string s;
    s = uppercase(get_xml(fname));

    if (debug) cout << "\ns=" << s << endl;

    string token = gettoken(s);
    int cnt = 0;

    while (s.length() > 0)
    {

      if (cnt++ > 5)
        return result;
      if (token == "")
        return result; 

      if (token == "<CELL>") {
        token = gettoken(s);
        continue; 
      }

      if (token == "<NAME>") {
        cellname = getvalue(s);
        token = gettoken(s);
      }

      if (token == "</NAME>") {
        token = gettoken(s);
      }


      if (token == "<CONTENTS>") {
        cellcontents = getvalue(s);
        token = gettoken(s);
        result.insert(pair<string, string>(cellname, cellcontents));
      }

      if (token == "</CONTENTS>") {
        token = gettoken(s);
      }


      if (token == "<SPREADSHEET>") {
        token = gettoken(s);
      }

      if (token == "</SPREADSHEET>") {
        break;
        token = gettoken(s);
      }

      if (debug) 
        cout << "\ns=" << s 
           << "\ntoken=" << token << "\ncellname=" 
           << cellname << "\ncellcontents=" 
           << cellcontents << endl
           << "\ncnt=" << cnt << endl;
    }
   
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
        if (debug) cout << c;
        result += c;
    }
    is.close();                

    if (result.empty()) 
      result = header;

    if (debug) cout << endl << endl << endl;

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


