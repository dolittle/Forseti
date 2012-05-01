# The What #
Forseti is a headless test / spec runner for JavaScript written to be independent of testing / specification frameworks and also independent of JavaScript engines. It has been built for keeping the feedback loop as tight as possible and has a high focus on conventions and automatic running of tests / specs.

# No browser #
Most test / spec runners out there have focus on bringing in real browsers and testing your JavaScript code against the real browsers. Although, we recognize the importance of this - we have a different angle to it and are saying that we want to optimize for the 99% usages when one is writing JavaScript code, instead of optimizing the the scenarios of running against a real browser. With this in mind, Forseti uses as its default the JavaScript engine directly and utilizes [env.js](http://www.envjs.com/) to fake the browser. This enables faster running of the tests / specs, closing the feedback loop. [Env.js](http://www.envjs.com/) does a great job simulating the browser and you are even able to still run your jQuery tests / specs.

# Getting started #

You can either build it yourself, or download a pre-compiled binary package from [here](https://github.com/downloads/dolittlestudios/Forseti/Forseti.zip). You will however have to have .net 4 installed on a Windows machine or Mono 2.8 installed on Mac OSX or Linux. The Forseti.exe file in the distribution is the one you want to run with Mono.  
  
A tip for Mono users is to set up Mono to by default run runtime version 4 in your .bash_profile file :

	export MONO_ENV_OPTIONS='--runtime=v4.0'


# How to build #

Forseti compiles fine in MonoDevelop 2 and Visual Studio 2010. Once you have the source, you navigate to the Source/Solutions path and you'll find two solution files, one that is tested under Windows and Visual Studio 2010 called *'Forseti.sln'* and another that has been tested with MonoDevelop 2 and Mac OSX; *'Forseti OSX.sln'*. If you're on OSX, you can't just go ahead and compile it, you need all the packages it depends on downloaded first. A rakefile has been created to do the downloads of these packages from [Nuget](http://www.nuget.org). 
From your terminal window :

	Your-Computer:Source someone$ sudo rake

Then you should be able to build it.
On Windows you only need to open the solution and build it with Visual Studio, it will automatically download the dependencies from Nuget.

# How to use #

For now, the only thing up and running is the console runner. It relies on a configuration file that you need to create called *forseti.yaml* - its layout is like this : 
	
	Harnesses:
		- Harness:
			Framework				: [name of testing/specification framework]
			Name					: [name of harness]
			SystemsSearchPath		: [relative path from current directory]/{placeholders}.js
			DescriptionsSearchPath	: [relative path from current directory]/{placeholders}.js
			Dependencies            :
			  - [relative path from current directory/harnessdependency.js]

A concrete configuration could be something like this : 

	Harnesses:
	  - Harness
          Framework					: Jasmine
          Name						: Something
          SystemsSearchPath			: Scripts/{system}.js
          DescriptionsSearchPath	: Specs/for_{system}/{description}.js
          Dependencies              :
            - Scripts/plugins/jquery.someplugin.js
			
Basically by configuring Forseti as above, you are setting up search paths that will represent your convention to where it finds systems you are testing / specifying and corresponding descriptions as we're calling it, but basically your tests / specs. You can create your own placeholders, there are for the time being no built in placeholders. You can also add another level if you're for instance using namespacing, you could easily have *'Scripts/{namespace}/{system}.js'*. For the time being, recursiveness in this mechanism is not supported. 

Once configured and Forseti is executed from the location of the Forseti.yaml file, it will start finding all systems and its corresponding tests / specs and then run everything that it has found. It will then sit there and wait till a new file is added, removed or modified and then run the impacted system and its corresponding tests / specs again.


### Dependencies ###

Forseti also supports multiple Harness', which can be useful when speccing different aspects of a system / multiple systems. In these cases there may be a mixed need for dependencies that can be resolved like so:

	Dependencies:
	  - Scripts/libs/jquery-1.7.2.min.js

	Harnesses:
	  - Harness
          Framework					: Jasmine
          Name						: Something
          SystemsSearchPath			: Scripts/{system}.js
          DescriptionsSearchPath	: Specs/for_{system}/{description}.js
          Dependencies              :
            - Scripts/plugins/jquery.someplugin.js

	  - Harness
          Framework					: Buster
          Name						: Something
          SystemsSearchPath			: Scripts/{system}.js
          DescriptionsSearchPath	: Specs/for_{system}/{description}.js
          Dependencies              :
            - Scripts/legacy/legacy.js
            - Scripts/legacy/ancient.js



# Frameworks #
Forseti supports multiple testing/specification frameworks. In the YAML file, you can for now specify either [Jasmine](http://pivotal.github.com/jasmine/) or [BusterJS](http://www.busterjs.org) as frameworks. But we are working on support of multiple others as well, amongst others [QUnit](http://docs.jquery.com/QUnit).

A sample of a Buster configuration : 

	Harnesses:
	  - Harness
	      Framework					: Buster
          Name						: Something
          SystemsSearchPath			: Scripts/{system}.js
          DescriptionsSearchPath	: Specs/for_{system}/{description}.js



# Why... Ohhh... Why... #

### Why another runner ###
Well. Looking at what was out there, we didn't feel that the philosophies of those projects were what we wanted. Feedback loop is really important and we want to optimize as mentioned for the 99% of the usage scenarios. The "edge cases" can be run in a browser context with other test runners on a continuous integration server, or a daily build - depending on how big feedback loop you can allow to have.

Another reason we wanted to create our own runner is that most runners was hard to get up and running properly, and also was very focused on the terminal window. Sure, the terminal is fine and one could probably argue that real developers live in the terminal. We think we can do better, so even though the terminal is one option of Forseti, in time we will have more options such as integration with popular IDEs.

### Why write it in .net ###
Well. The short answer; this is what we know best. Even though not considered the most cross platform friendly platforms, we have had great success with Mono.

# Future #
Please keep an eye open in the issues part of this site to see what we're working on, what features we're adding. Please contribute to the project through the issues list, or fork it and add features and send us a pull request. 
