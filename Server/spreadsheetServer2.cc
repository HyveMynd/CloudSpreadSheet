#include <cstdlib>
#include <iostream>
#include <boost/bind.hpp>
#include <boost/asio.hpp>
#include <boost/regex.hpp>
#include <stack>
#include <list>
#include <string>
#include "parse.h"
#include "ss_result.h"

using boost::asio::ip::tcp;
using namespace std; 

// represents a spreadsheets information
struct ssInfo
{
 string filename;
 bool open;
 string password;
};

class activeEditSession; // forword declaration

// global variables
std::map<string,ssInfo> mapOfExistingSS;
std::map<string,activeEditSession> mapOfActiveSS;

// represents one recent changes in a specific cell
struct editChange
{
  int value;
  string cell; // case sensitive

};

// class representing one editiing session , this class is used when somone joins a session
class activeEditSession
{
  private:
	int versionNumber;
	string fileName;
	list<tcp::socket> listOfUsers;
	stack<editChange> undoStack;
	map<string,string> cells;
  public:
	// constructor
 	activeEditSession(tcp::socket s, string f)
	{
	  //initialize version number
	  versionNumber = 0;
	  fileName = f;
	  //listOfSockets.add(tcp::socket);
	}
	// add another client's socket to editing session
	void addClient(tcp::socket s)
	{
	}
		
	// place functions for CHANGE, UNDO, END, LEAVE, SAVE, load
};

class socketConnection
{
public:
  socketConnection(boost::asio::io_service& io_service)
    : socket_(io_service)
  {
  }

  tcp::socket& socket()
  {
    return socket_;
  }

void start()
{
    std::string message_ = "Connection Established!\n";
    boost::asio::async_write(socket_,
      boost::asio::buffer(message_),        
        boost::bind(&socketConnection::connectionEstablished, this,
            boost::asio::placeholders::error));
/*
    socket_.async_read_some(boost::asio::buffer(data_, max_length),
        boost::bind(&socketConnection::handle_read, this,
          boost::asio::placeholders::error,
          boost::asio::placeholders::bytes_transferred));*/
  }

  // * method was originally handle write* this method is the callback to begin recieve and start
  // sends a message to the client
  void connectionEstablished(const boost::system::error_code& error)
  {
    if (!error)
    {	
	// begin recieve here with the callback being Recieve command
      socket_.async_read_some(boost::asio::buffer(data_, max_length),
          boost::bind(&socketConnection::RecieveCommand, this,
            boost::asio::placeholders::error,
            boost::asio::placeholders::bytes_transferred));
    }
    else
    {
      delete this;
    }
  }

  
  // place functions for CHANGE, UNDO, END, LEAVE, SAVE, load
  void RecieveCommand(const boost::system::error_code& error,
      size_t bytes_transferred)
  {
    string msg;
    if (!error)
    {
      // start parsing commands here 
      string command = data_;
      // output the clients command to the screen for testing purposes	
      cout << command << endl;

      
      // CREATES a file and add a file to the Spreadsheet map
      // if the file already exists or valid file name (no spaces)
      // responds with FAIL
      if(command.find("CREATE") != std::string::npos)
	{
	 string name = "";
         string password = "";
	 bool sendError = false;
         string message = "";
	 if(get_word(2,command,'\n').substr(0,5) == "Name:")
           {
		name = get_word(2,command,'\n').substr(5,(get_word(2,command,'\n')).size()-5);
           }
	 else
	      sendError = true;

	 if(get_word(3,command,'\n').substr(0,9) == "Password:")
           {
		password = get_word(3,command,'\n').substr(9,(get_word(3,command,'\n')).size()-9);
           }
	     sendError = true;
        if(sendError)	
           message = "ERROR\n";
        else
	{
          message = do_create(name,password).to_string();
	}
 	 boost::asio::async_write(socket_,
         boost::asio::buffer(message, bytes_transferred),
         boost::bind(&socketConnection::connectionEstablished, this,
            boost::asio::placeholders::error));
	}
      else if(command.find("JOIN") != std::string::npos)
	{
	 string message = "RECEIVED JOIN";
 	 boost::asio::async_write(socket_,
         boost::asio::buffer(message, bytes_transferred),
         boost::bind(&socketConnection::connectionEstablished, this,
            boost::asio::placeholders::error));
            // checks if the filename exists and that the password is correct in the filename dictionary. if it is,
            // add the users socket to an editing session - refer to example code here- and
            //  
	}
      else if(command.find("LEAVE") != std::string::npos)
	{
	  cout << "here" << endl;
	   //respond with 
	 string message = "LEAVE";
 	 boost::asio::async_write(socket_,
         boost::asio::buffer(message, bytes_transferred),
         boost::bind(&socketConnection::connectionEstablished, this,
            boost::asio::placeholders::error));
	}
    }
    else
    {
      delete this;
    }
  }
 
private:
  tcp::socket socket_;
  enum { max_length = 1024 };
  char data_[max_length];
};

class server
{
public:
	// constructor, starts the server running on port 1984
  server(boost::asio::io_service& io_service, short port)
    : io_service_(io_service),
      acceptor_(io_service, tcp::endpoint(tcp::v4(), port))
  {
    socketConnection* new_socketConnection = new socketConnection(io_service_);
    acceptor_.async_accept(new_socketConnection->socket(),
        boost::bind(&server::handle_accept, this, new_socketConnection,
          boost::asio::placeholders::error));
  }

  void handle_accept(socketConnection* new_socketConnection,
      const boost::system::error_code& error)
  {
    if (!error)
    {
      // start a new socketConnection
      new_socketConnection->start();
      new_socketConnection = new socketConnection(io_service_);
      // continue waiting for connections
      acceptor_.async_accept(new_socketConnection->socket(),
          boost::bind(&server::handle_accept, this, new_socketConnection,
            boost::asio::placeholders::error));
    }
    else
    {
      delete new_socketConnection;
    }
  }

private:
  boost::asio::io_service& io_service_;
  tcp::acceptor acceptor_;
};

int main()
{
  try
  {
   

    boost::asio::io_service io_service;

    server s(io_service, 1984);

    io_service.run();
  }
  catch (std::exception& e)
  {
    std::cerr << "Exception: " << e.what() << "\n";
  }

  return 0;
}

