ARCH = x86_64-w64-mingw32
PROGRAM = PiModK
EXEFILE = $(PROGRAM).exe
OBJFILES = DivisorSummatoryFunctionOdd.o FixedAllocator.o hr_time.o threadqueue.o $(PROGRAM).o
HFILES = stdafx.h DivisorSummatoryFunctionOdd.h FixedAllocator.h threadqueue.h stdafx.h
CC = $(ARCH)-gcc
CXX = $(ARCH)-g++
CXXFLAGS = -m64 -g -O2 -march=core2 -I/usr/local/include -L/usr/local/lib
#CXXFLAGS = -m64 -g -D_DEBUG -DDEBUG -march=core2 -I/usr/local/include -L/usr/local/lib
CFLAGS = $(CXXFLAGS)
LIBS = -lgmpxx -lgmp -lpthread

all: $(EXEFILE)

clean::
	rm -f $(OBJFILES) $(EXEFILE)

$(EXEFILE): $(OBJFILES)
	$(CXX) $(CXXFLAGS) $(OBJFILES) $(LIBS) -o $(EXEFILE)

$(OBJFILES): $(HFILES) Makefile
