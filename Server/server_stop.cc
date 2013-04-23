/* Client TCP Listener */

#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <netdb.h> 

#define BUFFER_LEN 256
void error(const char *msg)
{
      perror(msg);
          exit(0);
}

int main(int argc, char *argv[])
{
  int sockfd, portno, n;
  struct sockaddr_in serv_addr;
  struct hostent *server;
  char buffer[BUFFER_LEN];
  //char inputstr[100];

  FILE * input = NULL;

  
  if (argc < 3) {
    fprintf(stderr,"usage %s hostname port\n", argv[0]);
    exit(0);
  }
  portno = atoi(argv[2]);

      
  sockfd = socket(AF_INET, SOCK_STREAM, 0);

  if (sockfd < 0) 
    error("ERROR opening socket");

  server = gethostbyname(argv[1]);

  if (server == NULL) {
    fprintf(stderr,"ERROR, no such host\n");
    exit(0);
  }

  bzero((char *) &serv_addr, sizeof(serv_addr));
  serv_addr.sin_family = AF_INET;
  bcopy((char *)server->h_addr, (char *)&serv_addr.sin_addr.s_addr,
  server->h_length);
  serv_addr.sin_port = htons(portno);

  if (connect(sockfd,(struct sockaddr *) &serv_addr,sizeof(serv_addr)) < 0) 
    error("ERROR connecting");

  //printf("argc=%i\n", argc);
  //for (int i = 0; i < argc; i++)
    //printf("i=%i, %s\n", i, argv[i]);

  if (argc == 4) {
    printf("reading from file %s\n", argv[3]);
    input = fopen(argv[3],"r");
    if (input == NULL) {
      perror("Error opening file");
      exit(-1);
    }
    printf("file %s is open\n", argv[3]);
  }

  while (true) {
    // read from file or std input

    if (input != NULL) {
      bzero(buffer, BUFFER_LEN);
      printf("Reading from file %s\n", argv[3]);


      if (fgets(buffer, BUFFER_LEN, input)==NULL) {
        printf("fgets == NULL\n");
        break;;
      }
      //strcat(buffer,"\n");
      if (feof(input)) {
        printf("End of file \n");
        exit(0);
      }
    } else {
      printf("Shutting down the server");
      
      bzero(buffer,BUFFER_LEN);
      strcpy(buffer,"shutdown\n");
      //fgets(buffer,BUFFER_LEN,stdin);
      strcat(buffer,"\n");
    }

    n = write(sockfd,buffer,strlen(buffer));

    if (n < 0) 
      error("ERROR writing to socket");

    bzero(buffer,BUFFER_LEN);
    //n = read(sockfd,buffer,BUFFER_LEN);
    strcpy(buffer,"quit");

    if (strstr(buffer, "quit") > 0) {
        fprintf(stderr,"disconnected from server\n");
        break;
    }
    if (n < 0) 
      error("ERROR reading from socket");

    printf("%s\n",buffer);
  }

  close(sockfd);
  if (input != NULL)
    fclose(input);
  return 0;
}

