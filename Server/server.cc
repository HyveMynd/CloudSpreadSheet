<<<<<<< HEAD
/* A simple server in the internet domain using TCP
   The port number is passed as an argument */
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/types.h> 
#include <sys/socket.h>
#include <netinet/in.h>
#include <ostream>
#include <string>

using namespace std;

void error(const string msg)
{
    perror(msg.c_str());
    exit(1);
}


string openss(string fname, string username) {
  string str = fname + " " + username;
  return str;
}

string  updatess(const string filename, const string cell, const string data) {
  string str = "update " + filename + " " + cell + " " + data;
  return str;
}

string newss(const string filename,const string  password){
  return "newss " + filename + " " + password;
}


string closess(const string filename,const string  password){
  return "closess " + filename + " " + password;
}

int exitserver(int sockfd) {
  int n;
  string msg = "closing server socket\n";
  n = write(sockfd,msg.c_str(),msg.size());
}

void listen(int portno){

     int sockfd, newsockfd;
     socklen_t clilen;
     char buffer[256];
     struct sockaddr_in serv_addr, cli_addr;
     int n;
     string msg;


     sockfd = socket(AF_INET, SOCK_STREAM, 0);

     if (sockfd < 0) 
        error("ERROR opening socket");

     bzero((char *) &serv_addr, sizeof(serv_addr));

     serv_addr.sin_family = AF_INET;
     serv_addr.sin_addr.s_addr = INADDR_ANY;
     serv_addr.sin_port = htons(portno);

     if (bind(sockfd, (struct sockaddr *) &serv_addr, sizeof(serv_addr)) < 0) 
        error("ERROR on binding");

     listen(sockfd,5);
     printf("server listening on port %d \n", portno);

     clilen = sizeof(cli_addr);
     newsockfd = accept(sockfd, (struct sockaddr *) &cli_addr, &clilen);

     n = 1;

     while (n > 0) {

      if (newsockfd < 0) 
            error("ERROR on accept");

      bzero(buffer,256);
      //printf("before read \n");
      n = read(newsockfd,buffer,255);
      //  printf("after read\n");
    
      if (n < 0) 
        error("ERROR reading from socket");

      printf("Here is the message: %s\n",buffer);
      msg = "I got your message";

      if (strstr(buffer, "open") > 0) {
        msg = openss("test.ss","data");
      }
      else if (strstr(buffer, "quit") > 0) {
        int rc  = exitserver(newsockfd);
        printf("Exiting server \n");
        msg = "'quit' command received\n";
        break;
      }
      else if (strstr(buffer,"new") > 0) {
        msg = newss("filename","password");
      }
      else if (strstr(buffer,"close") > 0) {
        msg = closess("filename","password");
      }
      else if (strstr(buffer,"update") > 0) {
        msg = updatess("filename","cell","content");
      }
      else {
        msg = "I got your message";
      }
      msg += '\n';
     
      n = write(newsockfd,msg.c_str(),msg.size());

      if (n < 0) 
        error("ERROR writing to socket");
     }

     close(newsockfd);
     close(sockfd);
}

int main(int argc, char *argv[])
{
  if (argc < 2) {
    fprintf(stderr,"ERROR, no port provided\n");
    exit(1);
  }
  int portno;
  portno = atoi(argv[1]);
  listen(portno);
  return 0; 
}
=======
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
>>>>>>> added stubs for project
