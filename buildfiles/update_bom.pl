#!perl -w
###############################################################################
# Purpose:
#   This script was developed to create the release package for releases
#	This script was intended to be generic enough to be used with 
#	any project
#
# Requirements:
#   This scripts requires the PERL interpreter (ActivePerl)
# History:
#  20110428 - Micheal Schexnayder 
#	Initial creation
###############################################################################
# includes
use Cwd; # module for finding the current working directory

###############################################################################
# check for command line args
die ("Invalid number of arguments. Expected 1.\n") if ( @ARGV != 1 );

###############################################################################
# variables
my $BASEDIR = shift;
my $BOMFILE = "$BASEDIR/build_info.html";
my @MasterList; 

$BASEDIR =~ s/\\/\//g;

###############################################################################
# validate input

if ( ! -d $BASEDIR ){ 
	die ("$BASEDIR is NOT a valid directory\n");
}

if ( ! -f $BOMFILE ){
	die ("$BOMFILE does not exist!\n");
} 

###############################################################################
# script run

open (INFO, ">>$BOMFILE") || die ("Unable to open file: $BOMFILE"); #append mode
print INFO ('<table width="80%" align="center">');
print INFO ("\n<tr ><td ><div><b>Files included in this build:</div></td></tr>\n");

&ScanDirectory($BASEDIR); 

my $i = 0;
foreach $file (@MasterList){
	$file =~ s/$BASEDIR//;
	$i++;
	#print INFO ("<br>$i\)\t$file\n");
	print INFO ("<tr ><td ><div>$i\)\t$file</div></td></tr>\n");
}

###############################################################################
END {
	my $NumFiles = @MasterList;
	print ("Total number of files included in this patch: $NumFiles\n");
	print INFO ("</body>\n</html>");
	exit 0;
}
###############################################################################
# subroutines

sub ScanDirectory {
	my ($workdir) = shift; 
	my ($fname);

	my($startdir) = &cwd; # keep track of where we began

	chdir($workdir) or die "Unable to enter dir $workdir:$!\n";

	opendir(DIR, ".") or die "Unable to open $workdir:$!\n";
	my @names = readdir(DIR);
	closedir(DIR);
	 
	foreach my $name (@names){
		next if ($name eq "."); 
		next if ($name eq "..");
		next if ($name eq "build_info.html");
		
		if (-d $name){                     # is this a directory?
			&ScanDirectory($name);
			next;
		}
		
		if ( -f $name ){
			$fname = &cwd."/".$name;
			push (@MasterList, $fname);
		}
	}
	chdir($startdir) or die "Unable to change to dir $startdir:$!\n";
}

 