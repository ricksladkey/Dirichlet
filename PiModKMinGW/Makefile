PROGRAM = PiModK
EXEFILE = $(PROGRAM).exe
OBJFILES = DivisorSummatoryFunctionOdd.o FixedAllocator.o hr_time.o $(PROGRAM).o
HFILES = stdafx.h DivisorSummatoryFunctionOdd.h FixedAllocator.h stdafx.h
CXX = x86_64-w64-mingw32-g++
CXXFLAGS = -m64 -g -O2 -march=core2 -I/usr/local/include -L/usr/local/lib
#CXXFLAGS = -m64 -g -D_DEBUG -DDEBUG -march=core2 -I/usr/local/include -L/usr/local/lib
LDLIBS = -lgmpxx -lgmp

all: $(EXEFILE)

clean::
	rm -f $(OBJFILES) $(EXEFILE)

$(EXEFILE): $(OBJFILES)
	$(CXX) $(CXXFLAGS) $(OBJFILES) $(LDLIBS) -o PiModK.exe

$(OBJFILES): $(HFILES) Makefile

