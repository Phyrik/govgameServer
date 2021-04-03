$fromBranch = Read-Host "Which branch do you want to merge into master? (<branchname>/none/current)"

$currentBranch = git rev-parse --abbrev-ref HEAD
$allBranches = git for-each-ref --format='%(refname:short)' refs/heads/

switch ($fromBranch) {
	"none" {
		Break
	}
	"current" {
		git checkout master
		git merge $currentBranch
		git push
	}
	Default {
		git checkout master
		git merge $fromBranch
		git push
	}
}

foreach ($branch in $allBranches) {
	if ($branch -eq "master") { "Skipping master"; continue }
	git checkout $branch
	git merge master
	git push
}

git checkout $currentBranch