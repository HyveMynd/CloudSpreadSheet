std::string getword(int n, std::string line, ch : char)
{
  int word = 0;
  int i = 0;
  string s = "";
  while (line.length() > 0 && i < line.length() )
  {
    while (line[i] != ch && line[i] != '\0') {
    s = s + line[i];
    i++;
    }                                      }
    word++;
    if (word == n)
    {
       return s;
    }
    else s = "";
  }
}

