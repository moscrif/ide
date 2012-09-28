type: moscrif source code
name: Application
description: Empty native application skeleton
author: Jozef Prídavok & Moscrif, (C) 2010-212
param: application				text	app		^[a-zA-Z_][a-zA-Z0-9_]*$
################################################################################
; Date: $now$
; Author: $username$ on $computername$
           main: main.ms
          title: $application$
    description: 
         author: Moscrif
      copyright: 
       homepage: http://moscrif.com
           uses: core graphics uix crypto
    orientation: portrait
 remote-console: 
        version: 1.0
