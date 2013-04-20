all: controller 

controller: controller.cc controller.h parse.o cell.o user.o ss_result.o server.o
	g++ controller.cc -L server.o parse.o cell.o user.o ss_result.o -o controller

server.o: server.cc enums.h user.o spreadsheet.o ss_result.o cell.o
	g++ -c server.cc  

parse.o: parse.cc parse.h
	g++ -c parse.cc 

cell.o: cell.cc cell.h
	g++ -c cell.cc 

spreadsheet.o: spreadsheet.cc spreadsheet.h enums.h
	g++ -c spreadsheet.cc enums.h


user.o: user.cc user.h
	g++ -c user.cc

ss_result.o: ss_result.cc ss_result.h enums.h
	g++ -c ss_result.cc enums.h

spreadsheetServer2: spreadsheetServer2.cc parse.h ss_result.h 
	g++ spreadsheetServer2.cc parse.o ss_result.o -lboost_system -lpthread -o server2

test: test.cc parse.h parse.o
	g++ -o test test.cc parse.o parse.h

clean: 
	rm -f *.o test
