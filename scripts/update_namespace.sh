#!/bin/bash

# Function to update namespace in a file
update_namespace() {
    local filepath="$1"
    local directory_name=$(dirname "$filepath")
    local namespace=""

    # Remove the base directory and replace slashes with dots
    namespace="${directory_name#$PWD/}"
    namespace="${namespace//\//.}"

    # Remove trailing dot if present
    namespace="${namespace%.}"

    # Read the current content of the file
    current_content=$(cat "$filepath")

    # Extract the existing namespace using grep
    existing_namespace=$(echo "$current_content" | grep -oP 'namespace\s+\K[\w\.]+')

    if [[ -n "$existing_namespace" && "$existing_namespace" != "$namespace" ]]; then
        echo "Updating namespace in $filepath from $existing_namespace to $namespace"
        
        # Replace the existing namespace with the new one
        updated_content=$(echo "$current_content" | sed "s/namespace\s+$existing_namespace/namespace $namespace/")
        
        # Write the updated content back to the file
        echo "$updated_content" > "$filepath"
    else
        echo "No update needed for $filepath"
    fi
}

# Find all .cs files in the current directory and its subdirectories
find . -name "*.cs" -type f | while read -r filepath; do
    update_namespace "$filepath"
done