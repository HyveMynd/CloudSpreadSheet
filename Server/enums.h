//
//  commands.h
//  
//
//  Created by Andres Monroy on 4/17/13.
//
//

#ifndef _commands_h
#define _commands_h

#include <iostream>

namespace serverss {
    enum Command{
      Create,
        Join,
        Change,
        Update,
        Undo,
        Save,
        Leave
    };
    
    enum Status{
        OK,
        FAIL,
        WAIT,
        END
    };
}

#endif
