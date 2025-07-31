#!/bin/bash

# generate-scan-tests.sh
# Usage: ./generate-scan-tests.sh <input_directory>

set -e

# Get script directory for output file
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Default values
HOST="http://localhost:9999"
EGPATH="runtime/webhooks/EventGrid?functionName=ScanResultHandler"
CONTAINER="default-container"
BLOB_PATH_PREFIX="foo/bar/baz"
TOPIC="/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/defendercheck/providers/Microsoft.EventGrid/topics/fakedefenderchecktopic"

# Function to generate random UUID
generate_uuid() {
    if command -v uuidgen >/dev/null 2>&1; then
        uuidgen | tr '[:upper:]' '[:lower:]'
    else
        # Fallback for systems without uuidgen
        cat /proc/sys/kernel/random/uuid 2>/dev/null || \
        python3 -c "import uuid; print(str(uuid.uuid4()))" 2>/dev/null || \
        echo "$(date +%s | md5sum | cut -c1-8)-$(shuf -i 1000-9999 -n 1)-4$(shuf -i 1000-4999 -n 1)-$(shuf -i 8000-11999 -n 1)-$(shuf -i 100000000000-999999999999 -n 1)"
    fi
}

# Function to generate random eTag
generate_etag() {
    echo "0x$(openssl rand -hex 8 | tr '[:lower:]' '[:upper:]' 2>/dev/null || echo "$(shuf -i 10000000-99999999 -n 1)$(shuf -i 10000000-99999999 -n 1)" | tr '[:lower:]' '[:upper:]')"
}

# Function to generate timestamp
generate_timestamp() {
    # Generate timestamp in the format: 2025-05-04T20:57:41.1892174Z
    date -u +"%Y-%m-%dT%H:%M:%S.%7NZ" 2>/dev/null || date -u +"%Y-%m-%dT%H:%M:%S.$(shuf -i 1000000-9999999 -n 1)Z"
}

# Function to generate test for a single file
generate_test() {
    local filename="$1"
    local id=$(generate_uuid)
    local correlation_id=$(generate_uuid)
    local etag=$(generate_etag)
    local base_timestamp=$(generate_timestamp)
    
    cat << EOF

###
# ${filename}
POST {{host}}/{{egpath}} HTTP/1.1
...defaultHeaders
{
    "id": "${id}",
    "subject": "blobServices/default/containers/${CONTAINER}/blobs/${BLOB_PATH_PREFIX}/${filename}",
    "data": {
        "correlationId": "${correlation_id}",
        "blobUri": "http://127.0.0.1:10000/devstoreaccount1/${CONTAINER}/${BLOB_PATH_PREFIX}/${filename}",
        "eTag": "${etag}",
        "scanFinishedTimeUtc": "${base_timestamp}",
        "scanResultType": "No threats found",
        "scanResultDetails": null
    },
    "eventType": "Microsoft.Security.MalwareScanningResult",
    "dataVersion": "1.0",
    "metadataVersion": "1",
    "eventTime": "${base_timestamp}",
    "topic": "${TOPIC}"
}
EOF
}

# Function to generate random filename
generate_random_filename() {
    local timestamp=$(date +"%Y%m%d_%H%M%S")
    local random_suffix=$(shuf -i 1000-9999 -n 1)
    echo "generated-scan-tests-${timestamp}-${random_suffix}.http"
}

# Main script
main() {
    local input_dir="$1"
    
    # Validate input directory
    if [[ -z "$input_dir" ]]; then
        echo "Usage: $0 <input_directory>"
        echo "Example: $0 /path/to/files"
        exit 1
    fi
    
    if [[ ! -d "$input_dir" ]]; then
        echo "Error: Directory '$input_dir' does not exist"
        exit 1
    fi
    
    # Generate random output filename in script directory
    local output_file="${SCRIPT_DIR}/$(generate_random_filename)"
    
    # Count files to process
    local files=()
    while IFS= read -r -d '' file; do
        files+=("$file")
    done < <(find "$input_dir" -type f \( -name "*.csv" -o -name "*.fit" \) -print0 | sort -z)
    
    if [[ ${#files[@]} -eq 0 ]]; then
        echo "Warning: No .csv or .fit files found in '$input_dir'"
        exit 1
    fi
    
    echo "Generating tests for ${#files[@]} files from '$input_dir'"
    echo "Output file: '$output_file'"
    
    # Generate header (exactly matching your format)
    cat > "$output_file" << EOF
@host=${HOST}
@egpath=${EGPATH}

{{+
  exports.defaultHeaders = {
    'Content-Type': 'application/json',
    'Aeg-Event-Type': 'Notification'
  };
}}
EOF
    
    # Process each file
    for filepath in "${files[@]}"; do
        filename=$(basename "$filepath")
        generate_test "$filename" >> "$output_file"
        echo "Added test for: $filename"
        
        # Small delay to ensure different timestamps if needed
        sleep 0.01
    done
    
    echo "✅ Generated ${#files[@]} tests in '$output_file'"
    echo "📁 Output location: $output_file"
}

# Run main function with all arguments
main "$@"