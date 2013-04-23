#include <cstdlib>
#include <iostream>
#include <boost/bind.hpp>
#include <boost/asio.hpp>
#include <string>
#include "parse.h"
#include "ss_result.h"
#include "server.h"
#include "user.h"
#include "cell.h"
#include <stdlib.h>


using boost::asio::ip::tcp;
using namespace std;

namespace serverss
{
	//global variable
    
	class socketConnection
	{
    public:
	  server* my_server;
	  socketConnection(boost::asio::io_service& io_service, server* my_server)
	    : socket_(io_service)
        {
	  this->my_server = my_server;
            newUser = new user(&socket_);
	}
        
        tcp::socket& socket()
        {
            return socket_;
        }
        
		void start()
		{
			
		// begin recieve here with the callback being Recieve command
                socket_.async_read_some(boost::asio::buffer(data_, max_length),
                                        boost::bind(&socketConnection::RecieveCommand, this,
                                                    boost::asio::placeholders::error,
                                                    boost::asio::placeholders::bytes_transferred));

		}
        // * method was originally handle write* this method is the callback to all writes
        // this method begin receives from the client
        void connectionEstablished(const boost::system::error_code& error)
        {
            if (!error)
            {
		// empty data_
		memset(data_, 0, 1024);
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
        
        
        // place functions for CHANGE, UNDO, END, LEAVE, SAVE, UPDATE
        void RecieveCommand(const boost::system::error_code& error,
                            size_t bytes_transferred)
        {
            string command = data_;
	    cout << command << endl;
            string msg;
            if (!error)
            {
                // CREATES a file and add a file to the Spreadsheet map
                // if the file already exists or valid file name (no spaces)
                // responds with FAIL
                if(get_word(1,command,'\n').find("CREATE") != std::string::npos)
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
                    else
                        sendError = true;
                    
                    if(sendError)
                        message = "ERROR\n";
                    else
                    {
			message = (my_server->do_create(name,password)).to_string();
			//(my_server->do_create(name,password));
			cout << message << endl;
		    }
                    boost::asio::async_write(socket_,
                                             boost::asio::buffer(message, message.size()),
                                             boost::bind(&socketConnection::connectionEstablished, this,
                                                         boost::asio::placeholders::error));
                }
                else if(get_word(1,command,'\n').find("JOIN") != std::string::npos)
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
                    else
                        sendError = true;
                    
                    if(sendError)
                        message = "ERROR\n";
                    else
                    {
                        message_ = (my_server->do_join(name,password, newUser)).to_string();
                        cout << message_ << endl; 

                    }
                    
                    boost::asio::async_write(socket_,
                                             boost::asio::buffer(message_, message_.size()),
                                             boost::bind(&socketConnection::connectionEstablished, this,
                                                         boost::asio::placeholders::error));
                    
                }
                else if(get_word(1,command,'\n').find("CHANGE") != std::string::npos)
                {
                    string name = "";
                    string version = "";
                    string cellPos = "";
                    string length = "";
                    string content = "";
                    string message = "";
                    bool sendError = false;
                    
                    if(get_word(2,command,'\n').substr(0,5) == "Name:")
                    {
                        name = get_word(2,command,'\n').substr(5,(get_word(2,command,'\n')).size()-5);
                    }
                    else
                        sendError = true;
                    
                    if(get_word(3,command,'\n').substr(0,8) == "Version:")
                    {
                        version = get_word(3,command,'\n').substr(8,(get_word(3,command,'\n')).size()-8);
                    }
                    else
                        sendError = true;
                    
                    if(get_word(4,command,'\n').substr(0,5) == "Cell:")
			{
			cellPos = get_word(4,command,'\n').substr(5,(get_word(4,command,'\n')).size()-5);
			}
                    else
                        sendError = true;
                    
                    if(get_word(5,command,'\n').substr(0,7) == "Length:")
                    {
                        content = get_word(6,command,'\n');
                    }
                    else
                        sendError = true;
                    
                    if(sendError)
                        message = "ERROR\n";
                    else
                    {
                        message = (my_server->do_change(name,atoi(version.c_str()), cell(cellPos,content))).to_string();
                        cout << message << endl;
                    }
                    
                    boost::asio::async_write(socket_,
                                             boost::asio::buffer(message, message.size()),
                                             boost::bind(&socketConnection::connectionEstablished, this,
                                                         boost::asio::placeholders::error));
                    
                }
                else if(get_word(1,command,'\n').find("UNDO") != std::string::npos)
                {
                    string name = "";
                    string version = "";
                    string message = "";
                    bool sendError = false;
                    
                    if(get_word(2,command,'\n').substr(0,5) == "Name:")
                    {
                        name = get_word(2,command,'\n').substr(5,(get_word(2,command,'\n')).size()-5);
                    }
                    else
                        sendError = true;
                    
                    if(get_word(3,command,'\n').substr(0,8) == "Version:")
                    {
                        version = get_word(3,command,'\n').substr(8,(get_word(3,command,'\n')).size()-8);
                    }
                    else
                        sendError = true;
                    
                    if(sendError)
                        message = "ERROR\n";
					
                    else
                    {
                        message = (my_server->do_undo(name,atoi(version.c_str()))).to_string();
                    }
                    
                    boost::asio::async_write(socket_,
                                             boost::asio::buffer(message, message.size()),
                                             boost::bind(&socketConnection::connectionEstablished, this,
                                                         boost::asio::placeholders::error));
                }
                else if(get_word(1,command,'\n').find("SAVE") != std::string::npos)
                {
                    string name = "";
                    bool sendError = false;
                    string message = "";
                    if(get_word(2,command,'\n').substr(0,5) == "Name:")
                    {
                        name = get_word(2,command,'\n').substr(5,(get_word(2,command,'\n')).size()-5);
                    }
                    else
                        sendError = true;
                    
                    if(sendError)	
                        message = "ERROR\n";
                    else
                    {
                        message = (my_server->do_save(name)).to_string();
                    }
                    
                    boost::asio::async_write(socket_,
                                             boost::asio::buffer(message, message.size()),
                                             boost::bind(&socketConnection::connectionEstablished, this,
                                                         boost::asio::placeholders::error));
                }
                else if(command.find("LEAVE") != std::string::npos)
                {
                    string name = "";
                    bool sendError = false;
                    string message = "";
                    if(get_word(2,command,'\n').substr(0,5) == "Name:")
                    {
                        name = get_word(2,command,'\n').substr(5,(get_word(2,command,'\n')).size()-5);
                    }
                    else
                        sendError = true;
                    
                    if(sendError)	
                        message = "ERROR\n";
                    else
                    {	
                        my_server->do_leave(name, newUser);
                    }
                }
            }
            else
            {
                delete this;
            }
        }
        
	private:
	string message_;
        tcp::socket socket_;
        enum { max_length = 123456789 };
        char data_[max_length];
        // a user object 
        user* newUser;
	};
  
	class begin
	{
	public:
	  server* my_server;
		// constructor, starts the server running on port 1984
	  begin(boost::asio::io_service& io_service, short port, server* my_server)
		: io_service_(io_service),
        acceptor_(io_service, tcp::endpoint(tcp::v4(), port))
        {
	  this->my_server = my_server;
	  socketConnection* new_socketConnection = new socketConnection(io_service_, my_server);
            acceptor_.async_accept(new_socketConnection->socket(),
                                   boost::bind(&begin::handle_accept, this, new_socketConnection,
                                               boost::asio::placeholders::error));
        }
        
        void handle_accept(socketConnection* new_socketConnection,
                           const boost::system::error_code& error)
        {
            if (!error)
            {
                // start a new socketConnection
                new_socketConnection->start();
                new_socketConnection = new socketConnection(io_service_, my_server);
                // continue waiting for connections
                acceptor_.async_accept(new_socketConnection->socket(),
                                       boost::bind(&begin::handle_accept, this, new_socketConnection,
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
}

/*
 * main starts the server at a default port of 1984 unless 
 * read from command line 
 */
int main(int argc, char *argv[])
{
    int port = 1984;
    serverss::server* my_server = new serverss::server();


    try
    {
      if (argc > 1) {
        port = atoi(argv[1]);
      }
		  boost::asio::io_service io_service;

		  serverss::begin s(io_service, 1980, my_server);
		  io_service.run();
    }
      catch (std::exception& e)
    {
		  std::cerr << "Exception: " << e.what() << "\n";
    }

    delete my_server;
    return 0;
}

